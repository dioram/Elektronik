using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Extensions;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Parsers;
using Elektronik.Protobuf.Offline.Presenters;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayer : IDataSourcePlugin
    {
        public ProtobufFilePlayer(OfflineSettingsBag settings, ICSConverter converter)
        {
            _containerTree = new ProtobufContainerTree("Protobuf",
                                                       new FileImagePresenter("Camera", settings.PathToImagesDirectory),
                                                       new SlamDataInfoPresenter("Special info"));
            Data = _containerTree;
            _parsersChain = new DataParser<PacketPb>[]
            {
                new ObjectsParser(_containerTree.InfinitePlanes,
                                  _containerTree.Points,
                                  _containerTree.Observations,
                                  settings.PathToImagesDirectory),
                new TrackedObjectsParser(_containerTree.TrackedObjs),
                new InfoParser(_containerTree.SpecialInfo),
            }.BuildChain();
            
            _containerTree.DisplayName = $"Protobuf: {Path.GetFileName(settings.PathToFile)}";
            _input = File.OpenRead(settings.PathToFile);
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            _parsersChain.SetConverter(converter);

            _frames = new FramesCollection<Frame>(ReadCommands, TryGetSize());
            _threadWorker = new ThreadQueueWorker();
            _timer = new Timer(DelayBetweenFrames);
            _timer.Elapsed += (_, __) =>
            {
                _threadWorker.Enqueue(() =>
                {
                    if (GoToNextFrame()) return;
                    _timer.Stop();
                    MainThreadInvoker.Enqueue(() => Finished?.Invoke());
                });
            };
        }

        #region IDataSourceOffline

        public ISourceTree Data { get; }

        public void Dispose()
        {
            Data.Clear();
            _input.Dispose();
            _threadWorker.Dispose();
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        public int AmountOfFrames => _frames.CurrentSize;

        public string CurrentTimestamp => $"{_frames.Current?.Timestamp ?? 0} ({CurrentPosition})";

        public int CurrentPosition
        {
            get => _frames.CurrentIndex;
            set => RewindAt(value);
        }

        public int DelayBetweenFrames
        {
            get => _delayBetweenFrames;
            set
            {
                if (_delayBetweenFrames == value) return;

                _delayBetweenFrames = value;
                _timer.Interval = _delayBetweenFrames;
            }
        }

        public event Action<bool>? Rewind;
        public event Action? Finished;

        public void Play()
        {
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        public void StopPlaying()
        {
            _timer.Stop();
            _threadWorker.Enqueue(() =>
            {
                Data.Clear();
                _frames.SoftReset();
            });
        }

        public void PreviousKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (_frames.CurrentIndex == 0)
                {
                    _frames.Current?.Rewind();
                    _frames.SoftReset();
                    return;
                }
                do
                {
                    if (!GoToPreviousFrame()) break;
                } while (!(_frames.Current?.IsSpecial) ?? false);

            });
        }

        public void NextKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                do
                {
                    if (!GoToNextFrame()) break;
                } while (!(_frames.Current?.IsSpecial) ?? false);
            });
        }

        public void PreviousFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                if (_frames.CurrentIndex == 0)
                {
                    _frames.Current?.Rewind();
                    _frames.SoftReset();
                    return;
                }

                GoToPreviousFrame();
            });
        }
        
        public void NextFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                GoToNextFrame();
            });
        }

        #endregion

        #region Private definitions

        private readonly ProtobufContainerTree _containerTree;
        private readonly FileStream _input;
        private readonly FramesCollection<Frame> _frames;
        private readonly DataParser<PacketPb> _parsersChain;
        private readonly ThreadQueueWorker _threadWorker;
        private readonly Timer _timer;
        private int _delayBetweenFrames = 2;

        private IEnumerator<Frame> ReadCommands(int size)
        {
            if (size > 0)
            {
                for (var i = 0; i < size; i++)
                {
                    var packet = PacketPb.Parser.ParseDelimitedFrom(_input);
                    yield return Frame.ParsePacket(packet, _parsersChain);
                }
            }
            else
            {
                while (_input.Position < _input.Length)
                {
                    var packet = PacketPb.Parser.ParseDelimitedFrom(_input);
                    yield return Frame.ParsePacket(packet, _parsersChain);
                }
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

        private bool GoToPreviousFrame()
        {
            _frames.Current?.Rewind();
            if (_frames.MovePrevious())
            {
                (_containerTree.Image as FileImagePresenter)?.Present(_frames.Current);
                return true;
            }

            return false;
        }

        private bool GoToNextFrame()
        {
            if (_frames.MoveNext())
            {
                _frames.Current?.Show();
                (_containerTree.Image as FileImagePresenter)?.Present(_frames.Current);
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
                    if (_frames.CurrentIndex < pos) GoToNextFrame();
                    else GoToPreviousFrame();
                }
            });
        }

        #endregion
    }
}