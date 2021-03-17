using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using Elektronik.Ros.Rosbag.Parsers;
using Elektronik.Ros.RosMessages;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.Ros.Rosbag
{
    public class RosbagReader : DataSourceBase<FileScaleSettingsBag>, IDataSourceOffline
    {
        #region IDataSourceOffline

        public RosbagReader()
        {
            _container = new RosbagContainerTree("TMP");
            Data = _container;
            Finished += () => _playing = false;
        }

        public override string DisplayName => "ROS bag plugin";
        public override string Description => "This plugins allows Elektronik to read data saved from ROS.";

        public int AmountOfFrames => _frames?.CurrentSize ?? 0;
        public int CurrentTimestamp  => (int)((_frames?.Current?.Timestamp ?? 0 - _startTimestamp) / 1000000);
        public int CurrentPosition 
        {
            get => _frames?.CurrentIndex ?? 0;
            set => _rewindAt = value;
        }

        public event Action<bool>? Rewind;
        public event Action? Finished;

        public override void Start()
        {
            _container.Init(TypedSettings);
            _startTimestamp = _container.Parser?.ReadMessages().FirstOrDefault()?.Timestamp ?? 0;
            _frames = new FramesCollection<Frame>(ReadNext);
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);
            RosMessageConvertExtender.Converter = Converter;
            _threadWorker = new ThreadWorker();
        }

        public override void Stop()
        {
            _container.Reset();
            _threadWorker?.Dispose();
        }

        public override void Update(float delta)
        {
            if (_threadWorker != null && _threadWorker.QueuedActions != 0) return;
            if (_playing)
            {
                NextKeyFrame();
            }
            else if (_rewindAt > 0)
            {
                _playing = false;
                Rewind?.Invoke(true);
                if (CurrentPosition > _rewindAt)
                {
                    PreviousKeyFrame();
                }
                else if (CurrentPosition < _rewindAt)
                {
                    NextKeyFrame();
                }
                else
                {
                    _rewindAt = -1;
                    Rewind?.Invoke(false);
                }
            }
        }

        public void Play()
        {
            _playing = true;
        }

        public void Pause()
        {
            _playing = false;
        }

        public void StopPlaying()
        {
            _playing = false;
            _threadWorker?.Enqueue(() =>
            {
                Data.Clear();
                PresentersChain?.Clear();
                _frames?.SoftReset();
            });
        }

        public void PreviousKeyFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                _frames?.Current?.Rewind();
                if (!_frames?.MovePrevious() ?? false) _frames?.SoftReset();
            });
        }

        public void NextKeyFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                if (_frames?.MoveNext() ?? false)
                {
                    _frames.Current?.Show();
                }
                else
                {
                    MainThreadInvoker.Instance.Enqueue(Finished!);
                }
            });
        }

        #endregion

        #region Private

        private readonly RosbagContainerTree _container;
        private FramesCollection<Frame>? _frames;
        private ThreadWorker? _threadWorker;
        private bool _playing;
        private long _startTimestamp = 0;
        private int _rewindAt = -1;

        private IEnumerator<Frame> ReadNext()
        {
            return _container.Parser!
                    .ReadMessages()
                    .Where(m => m.TopicName is not null && m.TopicType is not null)
                    .Where(m => _container.RealChildren.Keys.Contains(m.TopicName))
                    .Select(m => (MessageParser.Parse(m.Data, m.TopicType!, false), _container.RealChildren[m.TopicName!], m.Timestamp))
                    .Where(data => data.Item1 is not null)
                    .Select(data => new Frame(data.Timestamp, data.Item1!.ToCommand(data.Item2)!))
                    .GetEnumerator();
        }
        
        #endregion
    }
}