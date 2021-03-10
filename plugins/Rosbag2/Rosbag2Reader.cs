using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Extensions;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using Elektronik.Presenters;
using Elektronik.Rosbag2.Data;
using Elektronik.Rosbag2.Parsers;
using Elektronik.Settings;
using UnityEngine;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.Rosbag2
{
    public class Rosbag2Reader : IDataSourceOffline
    {
        #region IDataSourceOffline implementation

        public string DisplayName => "Rosbag2 reader";
        public string Description => "Rosbag2 description";

        public SettingsBag Settings
        {
            get => _settings;
            set => _settings = (Rosbag2Settings) value;
        }

        public ISettingsHistory SettingsHistory { get; } = new Rosbag2SettingsHistory();

        public ICSConverter Converter { get; set; }

        public IContainerTree Data => _data;

        public DataPresenter PresentersChain { get; }

        public void Start()
        {
            _threadWorker = new ThreadWorker();
            _data.Init(_settings.DirPath);
            _startTimestamp = _data.DBModel.Table<Message>().OrderBy(m => m.Timestamp).First().Timestamp;
            AmountOfFrames = _data.DBModel.Table<Message>().Count();
            _messageParser = _data.GetRealChildren()
                    .OfType<TrackedObjectsContainer>()
                    .Select(c => (c, _data.DBModel.Table<Topic>().ToList().First(t => t.Name == _data.GetFullPath(c))))
                    .Select(d => new TrackedObjectsParser(d.c, d.Item2)).BuildChain();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            _messageParser.SetConverter(Converter);
            _frames = new FramesCollection<Frame>(ReadCommands, AmountOfFrames);
        }

        public void Stop()
        {
            Data.Clear();
            _threadWorker.Dispose();
        }

        public void Update(float delta)
        {
            if (!_playing) return;

            _timeout -= delta;
            if (_timeout < 0)
            {
                Task.Run(() => _threadWorker.Enqueue(() =>
                {
                    if (NextFrame()) return;

                    // getting timeout in seconds
                    var currTimestamp = _frames.Current.Timestamp;
                    if (NextFrame())
                    {
                        var nextTimestamp = _frames.Current.Timestamp;
                        PreviousFrame();
                        float nanoDelta = nextTimestamp - currTimestamp;
                        _timeout = nanoDelta / 1000000000;
                    }
                    else
                    {
                        _timeout = 0.001f;
                    }

                    MainThreadInvoker.Instance.Enqueue(() => Finished?.Invoke());
                    _playing = false;
                }));
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
            _threadWorker.Enqueue(() =>
            {
                Data.Clear();
                PresentersChain?.Clear();
                _frames.SoftReset();
            });
        }

        public void PreviousKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                do
                {
                    if (!PreviousFrame()) break;
                } while (_frames.Current.Command == null);
            });
        }

        public void NextKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                do
                {
                    if (!NextFrame()) break;
                } while (_frames.Current.Command == null);
            });
        }

        public int AmountOfFrames { get; private set; } = 0;
        public int CurrentTimestamp { get; private set; }

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                if (_currentPosition == value) return;
                RewindAt(value);
            }
        }

        public event Action Finished;

        #endregion

        #region Private definitions

        private Rosbag2Settings _settings = new Rosbag2Settings();
        private readonly Rosbag2ContainerTree _data = new Rosbag2ContainerTree("TMP");
        private DataParser<(Message, Topic)> _messageParser;
        private FramesCollection<Frame> _frames;
        private ThreadWorker _threadWorker;
        private long _startTimestamp;
        private bool _playing;
        private float _timeout;
        private int _currentPosition;
        private bool _rewinding = false;

        private bool PreviousFrame()
        {
            var curr = _frames.Current;
            if (_frames.MovePrevious())
            {
                curr.Rewind();
                PresentersChain?.Present(_frames.Current);
                _currentPosition = _frames.CurrentIndex;
                CurrentTimestamp = (int) (_frames.Current.Timestamp / 1000);
                return true;
            }

            return false;
        }

        private bool NextFrame()
        {
            if (_frames.MoveNext())
            {
                var next = _frames.Current;
                next.Show();
                PresentersChain?.Present(next);
                _currentPosition = _frames.CurrentIndex;
                CurrentTimestamp = (int) (_frames.Current.Timestamp / 1000);
                return true;
            }

            return false;
        }

        private IEnumerator<Frame> ReadCommands(bool _)
        {
            long lastTimestamp = long.MinValue;
            while (true)
            {
                var timestamp = lastTimestamp;
                var message = _data.DBModel.Table<Message>()
                        .Where(m => m.Timestamp > timestamp)
                        .OrderBy(m => m.Timestamp)
                        .FirstOrDefault();
                lastTimestamp = message?.Timestamp ?? lastTimestamp;
                if (message == null) yield break;

                var topic = _data.DBModel.Table<Topic>().First(t => t.Id == message.TopicID);
                yield return Frame.ParseMessage(lastTimestamp - _startTimestamp, message, topic, _messageParser);
            }
        }

        private void RewindAt(int pos)
        {
            if (pos < 0 || pos >= AmountOfFrames || pos == CurrentPosition || _rewinding) return;

            _threadWorker.Enqueue(() =>
            {
                _rewinding = true;
                while (_frames.CurrentIndex != pos)
                {
                    if (_frames.CurrentIndex < pos) NextFrame();
                    else PreviousFrame();
                }

                _rewinding = false;
            });
        }

        #endregion
    }
}