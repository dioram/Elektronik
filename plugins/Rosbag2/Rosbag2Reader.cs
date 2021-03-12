using System;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.Presenters;
using Elektronik.Rosbag2.Containers;
using Elektronik.Rosbag2.Parsers;
using Elektronik.Settings;
using UnityEngine;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.Rosbag2
{
    public class Rosbag2Reader : IDataSourceOffline
    {
        public Rosbag2Reader()
        {
            var sh = new Rosbag2SettingsHistory();
            SettingsHistory = sh;
            if (sh.Recent.Count > 0) _settings = (Rosbag2Settings) sh.Recent[0];
            else _settings = new Rosbag2Settings();
        }

        #region IDataSourceOffline implementation

        public string DisplayName => "Rosbag2 reader";
        public string Description => "This plugins allows Elektronik to read data saved from ROS2.";

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

            _actualTimestamps = _data.GetRealChildren()
                    .OfType<IDBContainer>()
                    .SelectMany(c => c.ActualTimestamps)
                    .OrderBy(i => i)
                    .ToArray();

            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            RosMessageConvertExtender.Converter = Converter;
        }

        public void Stop()
        {
            _data.Reset();
            _threadWorker.Dispose();
        }

        public void Update(float delta)
        {
            if (_threadWorker.QueuedActions != 0) return;
            if (_playing)
            {
                if (CurrentPosition == AmountOfFrames - 1)
                {
                    MainThreadInvoker.Instance.Enqueue(() => Finished?.Invoke());
                    _playing = false;
                    return;
                }

                NextKeyFrame();
            }
            else if (_rewindPlannedPos > 0)
            {
                _currentPosition = _rewindPlannedPos;
                _rewindPlannedPos = -1;
                _threadWorker.Enqueue(() => { _data.ShowAt(_actualTimestamps[_currentPosition], true); });
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
                _currentPosition = 0;
                Data.Clear();
                PresentersChain?.Clear();
            });
        }

        public void PreviousKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (CurrentPosition == 0) return;
                _currentPosition--;
                _data.ShowAt(_actualTimestamps[CurrentPosition]);
            });
        }

        public void NextKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (CurrentPosition == AmountOfFrames - 1) return;

                _currentPosition++;
                _data.ShowAt(_actualTimestamps[CurrentPosition]);
            });
        }

        public int AmountOfFrames => _actualTimestamps?.Length ?? 0;

        public int CurrentTimestamp =>
                (int) ((_actualTimestamps?[CurrentPosition] - _actualTimestamps?[0] ?? 0) / 1000);

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                if (value < 0 || value >= AmountOfFrames || _currentPosition == value) return;
                _rewindPlannedPos = value;
                _playing = false;
            }
        }

        public event Action Finished;

        #endregion

        #region Private definitions

        private Rosbag2Settings _settings = new Rosbag2Settings();
        private readonly Rosbag2ContainerTree _data = new Rosbag2ContainerTree("TMP");
        private ThreadWorker _threadWorker;
        private bool _playing;
        private int _currentPosition;
        private long[] _actualTimestamps;
        private int _rewindPlannedPos;

        #endregion
    }
}