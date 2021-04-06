﻿using System;
using System.Linq;
using Elektronik.PluginsSystem;
using Elektronik.Rosbag2.Containers;
using Elektronik.Rosbag2.Parsers;
using UnityEngine;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.Rosbag2
{
    public class Rosbag2Reader : DataSourceBase<Rosbag2Settings>, IDataSourceOffline
    {
        public Rosbag2Reader()
        {
            _data = new Rosbag2ContainerTree("TMP");
            Data = _data;
        }
        
        #region IDataSourceOffline implementation

        public override string DisplayName => "Rosbag2 reader";
        public override string Description => "This plugins allows Elektronik to read data saved from ROS2.";

        public override void Start()
        {
            _threadWorker = new ThreadWorker();
            _data.Init(TypedSettings.DirPath);

            _actualTimestamps = _data.GetRealChildren()
                    .OfType<IDBContainer>()
                    .SelectMany(c => c.ActualTimestamps)
                    .OrderBy(i => i)
                    .ToArray();

            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            RosMessageConvertExtender.Converter = Converter;
        }

        public override void Stop()
        {
            _data.Reset();
            _threadWorker.Dispose();
        }

        public override void Update(float delta)
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

        private readonly Rosbag2ContainerTree _data;
        private ThreadWorker _threadWorker;
        private bool _playing;
        private int _currentPosition;
        private long[] _actualTimestamps;
        private int _rewindPlannedPos;

        #endregion
    }
}