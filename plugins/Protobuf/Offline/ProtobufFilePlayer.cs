﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Elektronik.Extensions;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using Elektronik.Presenters;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Parsers;
using Elektronik.Protobuf.Offline.Presenters;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayer : DataSourceBase<OfflineSettingsBag>, IDataSourceOffline
    {
        public const float Timeout = 0.5f;

        public ProtobufFilePlayer()
        {
            _containerTree = new ProtobufContainerTree("Protobuf");
            Data = _containerTree;
            _parsersChain = new DataParser<PacketPb>[]
            {
                new ObjectsParser(_containerTree.InfinitePlanes,
                                  _containerTree.Points,
                                  _containerTree.Observations,
                                  TypedSettings.ImagePath),
                new TrackedObjectsParser(_containerTree.TrackedObjs),
                new InfoParser(),
            }.BuildChain();

            PresentersChain = new DataPresenter[]
            {
                new ImagePresenter(TypedSettings),
                new SlamDataInfoPresenter(_containerTree.Points, _containerTree.Observations),
            }.BuildChain();
        }

        #region IDataSourceOffline

        public override string DisplayName => "Protobuf";

        public override string Description => "Reads protobuf packages from file.";

        public override void Start()
        {
            _containerTree.DisplayName = $"Protobuf: {Path.GetFileName(TypedSettings.FilePath)}";
            _input = File.OpenRead(TypedSettings.FilePath!);
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);
            _parsersChain.SetConverter(Converter);

            _frames = new FramesCollection<Frame>(ReadCommands, TryGetSize());
            _threadWorker = new ThreadWorker();
        }

        public override void Stop()
        {
            Data.Clear();
            _input.Dispose();
            _threadWorker.Dispose();
        }

        public override void Update(float delta)
        {
            if (!_playing) return;

            _timeout -= delta;
            if (_timeout < 0)
            {
                Task.Run(() => _threadWorker.Enqueue(() =>
                {
                    if (NextFrame()) return;
                    MainThreadInvoker.Instance.Enqueue(() => Finished?.Invoke());
                    _playing = false;
                }));
                _timeout = Timeout;
            }
        }

        public int AmountOfFrames => _frames?.CurrentSize ?? 0;

        public int CurrentTimestamp => _frames?.Current?.Timestamp ?? 0;

        public int CurrentPosition
        {
            get => _frames?.CurrentIndex ?? 0;
            set => RewindAt(value);
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
                PresentersChain.Clear();
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
                } while (!_frames.Current.IsSpecial);
            });
        }

        public void NextKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                do
                {
                    if (!NextFrame()) break;
                } while (!_frames.Current.IsSpecial);
            });
        }

        public event Action Finished;

        #endregion

        #region Private definitions

        private readonly ProtobufContainerTree _containerTree;
        private FileStream _input;
        private FramesCollection<Frame> _frames;
        private readonly DataParser<PacketPb> _parsersChain;
        private bool _playing = false;
        private float _timeout = 0;
        private ThreadWorker _threadWorker;

        private const int MetadataOffset = 8;

        private IEnumerator<Frame> ReadCommands(bool isSizeKnown)
        {
            var length = _input.Length - (isSizeKnown ? MetadataOffset : 0);
            while (_input.Position < length)
            {
                var packet = PacketPb.Parser.ParseDelimitedFrom(_input);
                yield return Frame.ParsePacket(packet, _parsersChain);
            }

            _input.Dispose();
        }

        private int TryGetSize()
        {
            _input.Position = _input.Length - 8;
            var buffer = new byte[4];
            _input.Read(buffer, 0, 4);
            var marker = BitConverter.ToUInt32(buffer, 0);
            if (marker == 0xDEADBEEF)
            {
                _input.Read(buffer, 0, 4);
                _input.Position = 0;
                return BitConverter.ToInt32(buffer, 0);
            }

            _input.Position = 0;
            return 0;
        }

        private bool PreviousFrame()
        {
            var curr = _frames.Current;
            if (_frames.MovePrevious())
            {
                curr.Rewind();
                PresentersChain.Present(_frames.Current);
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
                PresentersChain.Present(next);
                return true;
            }

            return false;
        }

        private void RewindAt(int pos)
        {
            if (pos < 0 || pos >= AmountOfFrames || pos == CurrentPosition) return;

            _threadWorker.Enqueue(() =>
            {
                while (_frames.CurrentIndex != pos)
                {
                    if (_frames.CurrentIndex < pos) NextFrame();
                    else PreviousFrame();
                }
            });
        }

        #endregion
    }
}