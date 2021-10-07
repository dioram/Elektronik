using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataSources;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.RosPlugin.Ros.Bag.Parsers.Records;
using Elektronik.Settings;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagReader : IRewindableDataSource
    {
        public RosbagReader(string displayName, Texture2D? logo, RosbagSettings settings)
        {
            Logo = logo;
            DisplayName = displayName;
            _container = new RosbagContainerTree(settings, "TMP");
            Data = _container;
            OnFinished += Pause;
            _startTimestamp = _container.Parser.ReadMessagesAsync().FirstOrDefaultAsync().Result?.Timestamp ?? 0;
            var actualConnections = _container.Parser.GetTopics()
                    .Where(t => _container.ActualTopics.Select(a => a.Name).Contains(t.Topic));
            _frames = new FramesAsyncCollection<Frame>(() => ReadNext(actualConnections));
            _frames.OnCurrentSizeChanged += i => OnAmountOfFramesChanged?.Invoke(i);
            var converter = new RosConverter();
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = converter;
            _threadWorker = new ThreadQueueWorker();
        }

        #region IDataSourcePlugin

        public ISourceTreeNode Data { get; }

        public int AmountOfFrames => _frames.CurrentSize;
        public bool IsPlaying { get; private set; }
        public event Action? OnPlayingStarted;
        public event Action? OnPaused;
        public event Action<int>? OnPositionChanged;
        public event Action<int>? OnAmountOfFramesChanged;
        public event Action<string>? OnTimestampChanged;
        public event Action? OnRewindStarted;
        public event Action? OnRewindFinished;
        public event Action? OnFinished;

        public string Timestamp
        {
            get
            {
                var t = _frames.Current?.Timestamp ?? 0; // One who designed this time storage format is really weird.
                var s = _startTimestamp;                 // Instead of saving time as 1 long or ulong value
                var ts = (int)t; // secs                      // or saving 2 separate int values
                var tn = (int)(t >> 32); // nanosecs          // He took 2 ints and store them inside 1 long value.
                var ss = (int)s; // secs                      // WTF???
                var sn = (int)(s >> 32); // nanosecs
                t = ts * 1000 + tn / 1000000;
                s = ss * 1000 + sn / 1000000;
                return $"{(t - s) / 1000f:F3}";
            }
        }

        public int Position
        {
            get => _frames.CurrentIndex;
            set
            {
                if (value < 0 || value >= AmountOfFrames || Position == value) return;
                _rewindAt = value;
            }
        }

        public void Dispose()
        {
            _container.Dispose();
            _threadWorker.Dispose();
        }

        public void Update(float delta)
        {
            if (IsPlaying)
            {
                NextKeyFrame();
            }
            else if (_rewindAt > 0)
            {
                IsPlaying = false;
                OnRewindStarted?.Invoke();
                if (Position > _rewindAt)
                {
                    PreviousKeyFrame();
                }
                else if (Position < _rewindAt)
                {
                    NextKeyFrame();
                }
                else
                {
                    _rewindAt = -1;
                    OnRewindFinished?.Invoke();
                }
            }
        }

        public string DisplayName { get; }
        public SettingsBag? Settings => null;
        public Texture2D? Logo { get; }

        public void Play()
        {
            IsPlaying = true;
            OnPlayingStarted?.Invoke();
        }

        public void Pause()
        {
            IsPlaying = false;
            OnPaused?.Invoke();
        }

        public void StopPlaying()
        {
            IsPlaying = false;
            OnPaused?.Invoke();
            _threadWorker.Enqueue(() =>
            {
                Data.Clear();
                _frames.SoftReset();
            });
        }

        public void PreviousKeyFrame() => PreviousFrame();

        public void NextKeyFrame() => NextFrame();

        public void PreviousFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                _frames.Current?.Rewind();
                if (!_frames.MovePrevious()) _frames.SoftReset();
                OnPositionChanged?.Invoke(Position);
                OnTimestampChanged?.Invoke(Timestamp);
            });
        }

        public void NextFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (_frames.MoveNext())
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    _frames.Current?.Show();
                }
                else
                {
                    OnFinished?.Invoke();
                }
                OnPositionChanged?.Invoke(Position);
                OnTimestampChanged?.Invoke(Timestamp);
            });
        }

        #endregion

        #region Private

        private readonly RosbagContainerTree _container;
        private readonly FramesAsyncCollection<Frame> _frames;
        private readonly ThreadQueueWorker _threadWorker;
        private readonly long _startTimestamp;
        private int _rewindAt = -1;

        private IAsyncEnumerator<Frame> ReadNext(IEnumerable<Connection>? topics)
        {
            return _container.Parser
                    .ReadMessagesAsync(topics)
                    .Where(m => m.TopicName is not null && m.TopicType is not null)
                    .Where(m => _container.RealChildren.Keys.Contains(m.TopicName))
                    .Select(m => (MessageParser.Parse(m.Data!, m.TopicType!, false),
                                  _container.RealChildren[m.TopicName!], m.Timestamp))
                    .Where(data => data.Item1 is not null)
                    .Select(data => new Frame(data.Timestamp, data.Item1!.ToCommand(data.Item2)))
                    .GetAsyncEnumerator();
        }

        #endregion
    }
}