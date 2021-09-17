using System;
using System.Linq;
using Elektronik.DataSources;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Containers;
using Elektronik.Settings.Bags;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class Rosbag2Reader : IDataSourcePlugin
    {
        public Rosbag2Reader(string displayName, Texture2D? logo, Rosbag2Settings settings)
        {
            DisplayName = displayName;
            Logo = logo;
            _data = new Rosbag2ContainerTree(settings);
            Data = _data;
            _threadWorker = new ThreadQueueWorker();

            _actualTimestamps = _data.Timestamps.Values
                    .SelectMany(l => l)
                    .OrderBy(i => i)
                    .ToArray();

            var converter = new RosConverter();
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = converter;
        }

        #region IDataSourcePlugin

        public ISourceTreeNode Data { get; }

        public void Dispose()
        {
            _data.Dispose();
            _threadWorker.Dispose();
        }

        public void Update(float delta)
        {
            if (_playing)
            {
                if (Position == AmountOfFrames - 1)
                {
                    MainThreadInvoker.Enqueue(() => OnFinished?.Invoke());
                    _playing = false;
                    return;
                }

                NextKeyFrame();
            }
            else if (_rewindPlannedPos > 0)
            {
                OnRewindStarted?.Invoke();
                _currentPosition = _rewindPlannedPos;
                _rewindPlannedPos = -1;
                _threadWorker.Enqueue(() =>
                {
                    _data.ShowAt(_actualTimestamps[_currentPosition], true);
                    OnRewindFinished?.Invoke();
                });
            }
        }

        public string DisplayName { get; }
        public SettingsBag? Settings => null;
        public Texture2D? Logo { get; }

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
            });
        }

        public void PreviousKeyFrame() => PreviousFrame();

        public void NextKeyFrame() => NextFrame();

        public void PreviousFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (Position == 0) return;
                _currentPosition--;
                _data.ShowAt(_actualTimestamps[Position]);
            });
        }

        public void NextFrame()
        {
            if (_threadWorker.ActiveActions > 0) return;
            _threadWorker.Enqueue(() =>
            {
                if (Position == AmountOfFrames - 1) return;

                _currentPosition++;
                _data.ShowAt(_actualTimestamps[Position]);
            });
        }

        public int AmountOfFrames => _actualTimestamps.Length;

        public string Timestamp =>
                $"{(_actualTimestamps[Position] - _actualTimestamps[0]) / 1000000000f:F3}";

        public int Position
        {
            get => _currentPosition;
            set
            {
                if (value < 0 || value >= AmountOfFrames || _currentPosition == value) return;
                _rewindPlannedPos = value;
                _playing = false;
            }
        }

        public float Speed { get; set; }
        public bool IsPlaying { get; }
        public event Action? OnPlayingStarted;
        public event Action? OnPaused;
        public event Action<int>? OnPositionChanged;
        public event Action<int>? OnAmountOfFramesChanged;
        public event Action<string>? OnTimestampChanged;

        public event Action? OnRewindStarted;
        public event Action? OnRewindFinished;
        public event Action? OnFinished;

        #endregion

        #region Private definitions

        private readonly Rosbag2ContainerTree _data;
        private readonly ThreadQueueWorker _threadWorker;
        private bool _playing;
        private int _currentPosition;
        private readonly long[] _actualTimestamps;
        private int _rewindPlannedPos;

        #endregion
    }
}