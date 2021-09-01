using System;
using System.Linq;
using Elektronik.Data;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Containers;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag
{
    public class Rosbag2Reader : IDataSourcePluginOffline
    {
        public Rosbag2Reader(Rosbag2Settings settings)
        {
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

        #region IDataSourceOffline

        public ISourceTree Data { get; }

        public void Dispose()
        {
            _data.Dispose();
            _threadWorker.Dispose();
        }

        public void Update(float delta)
        {
            if (_playing)
            {
                if (CurrentPosition == AmountOfFrames - 1)
                {
                    MainThreadInvoker.Enqueue(() => Finished?.Invoke());
                    _playing = false;
                    return;
                }

                NextKeyFrame();
            }
            else if (_rewindPlannedPos > 0)
            {
                Rewind?.Invoke(true);
                _currentPosition = _rewindPlannedPos;
                _rewindPlannedPos = -1;
                _threadWorker.Enqueue(() =>
                {
                    _data.ShowAt(_actualTimestamps[_currentPosition], true);
                    Rewind?.Invoke(false);
                });
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
            });
        }

        public void PreviousKeyFrame() => PreviousFrame();

        public void NextKeyFrame() => NextFrame();

        public void PreviousFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (CurrentPosition == 0) return;
                _currentPosition--;
                _data.ShowAt(_actualTimestamps[CurrentPosition]);
            });
        }

        public void NextFrame()
        {
            if (_threadWorker.ActiveActions > 0) return;
            _threadWorker.Enqueue(() =>
            {
                if (CurrentPosition == AmountOfFrames - 1) return;

                _currentPosition++;
                _data.ShowAt(_actualTimestamps[CurrentPosition]);
            });
        }

        public int AmountOfFrames => _actualTimestamps.Length;

        public string CurrentTimestamp =>
                $"{(_actualTimestamps[CurrentPosition] - _actualTimestamps[0]) / 1000000000f:F3}";

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

        public int DelayBetweenFrames { get; set; }

        public event Action<bool>? Rewind;

        public event Action? Finished;

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