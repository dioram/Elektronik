using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.RosPlugin.Ros.Bag.Parsers.Records;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class RosbagReader : DataSourcePluginBase<RosbagSettings>, IDataSourcePluginOffline
    {
        #region IDataSourceOffline

        public RosbagReader()
        {
            _container = new RosbagContainerTree("TMP");
            Data = _container;
            Finished += () => _playing = false;
        }

        public override string DisplayName => "ROS bag";

        public override string Description => "This plugins allows Elektronik to read data saved from " +
                "<#7f7fe5><u><link=\"https://www.ros.org\">ROS</link></u></color>" +
                " using <#7f7fe5><u><link=\"http://wiki.ros.org/rosbag\">rosbag</link></u></color>.";

        public void SetFileName(string filename)
        {
            TypedSettings.FilePath = filename;
        }

        public int AmountOfFrames => _frames?.CurrentSize ?? 0;

        public string CurrentTimestamp
        {
            get
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                var t = _frames?.Current?.Timestamp ?? 0; // I want to kill man who designed this time storage format.
                var s = _startTimestamp;                  // Instead of saving time as 1 long or ulong value
                var ts = (int) t; // secs                      // or saving 2 separate int values
                var tn = (int) (t >> 32); // nanosecs          // He took 2 ints and store them inside 1 long value.
                var ss = (int) s; // secs                      // WTF???
                var sn = (int) (s >> 32); // nanosecs
                t = ts * 1000 + tn / 1000000;
                s = ss * 1000 + sn / 1000000;
                return $"{(t-s)/1000f:F3}";
            }
        }

        public string[] SupportedExtensions { get; } = {".bag"};

        public int CurrentPosition
        {
            get => _frames?.CurrentIndex ?? 0;
            set
            {
                if (value < 0 || value >= AmountOfFrames || CurrentPosition == value) return;
                _rewindAt = value;
            }
        }

        public int DelayBetweenFrames { get; set; }

        public event Action<bool>? Rewind;
        public event Action? Finished;

        public override void Start()
        {
            _container.Init(TypedSettings);
            _startTimestamp = _container.Parser?.ReadMessagesAsync().FirstOrDefaultAsync().Result?.Timestamp ?? 0;
            var actualConnections = _container.Parser?
                    .GetTopics()
                    .Where(t => _container.ActualTopics.Select(a => a.Name).Contains(t.Topic));
            _frames = new FramesAsyncCollection<Frame>(() => ReadNext(actualConnections));
            Converter = new RosConverter();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = Converter;
            _threadWorker = new ThreadQueueWorker();
        }

        public override void Stop()
        {
            _container.Reset();
            _threadWorker?.Dispose();
        }

        public override void Update(float delta)
        {
            if (_threadWorker == null/* || _threadWorker.AmountOfActions > 0*/) return;
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
                _frames?.SoftReset();
            });
        }

        public void PreviousKeyFrame() => PreviousFrame();

        public void NextKeyFrame() => NextFrame();

        public void PreviousFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                _frames?.Current?.Rewind();
                if (!_frames?.MovePrevious() ?? false) _frames?.SoftReset();
            });
        }

        public void NextFrame()
        {
            _threadWorker?.Enqueue(() =>
            {
                if (_frames?.MoveNext() ?? false)
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    _frames.Current?.Show();
                }
                else
                {
                    MainThreadInvoker.Enqueue(Finished!);
                }
            });
        }
        
        #endregion

        #region Private

        private readonly RosbagContainerTree _container;
        private FramesAsyncCollection<Frame>? _frames;
        private ThreadQueueWorker? _threadWorker;
        private bool _playing;
        private long _startTimestamp = 0;
        private int _rewindAt = -1;

        private IAsyncEnumerator<Frame> ReadNext(IEnumerable<Connection>? topics)
        {
            return _container.Parser!
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