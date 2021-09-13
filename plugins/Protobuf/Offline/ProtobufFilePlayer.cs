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
using Elektronik.Settings.Bags;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayer : IDataSourcePlugin
    {
        public ProtobufFilePlayer(string displayName, Texture2D? logo, OfflineSettingsBag settings,
                                  ICSConverter converter)
        {
            _containerTree = new ProtobufContainerTree("Protobuf",
                                                       new FileImagePresenter("Camera", settings.PathToImagesDirectory),
                                                       new SlamDataInfoPresenter("Special info"));
            Data = _containerTree;
            DisplayName = displayName;
            Logo = logo;
            _parsersChain = new DataParser<PacketPb>[]
            {
                new ObjectsParser(_containerTree.InfinitePlanes, _containerTree.Points, _containerTree.Observations,
                                  settings.PathToImagesDirectory),
                new TrackedObjectsParser(_containerTree.TrackedObjs),
                new InfoParser(_containerTree.SpecialInfo),
            }.BuildChain();

            _containerTree.DisplayName = $"Protobuf: {Path.GetFileName(settings.PathToFile)}";
            _input = File.OpenRead(settings.PathToFile);
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            _parsersChain.SetConverter(converter);

            _frames = new FramesCollection<Frame>(ReadCommands, TryGetSize());
            _frames.OnSizeChanged += i => OnAmountOfFramesChanged?.Invoke(i);
            _threadWorker = new ThreadQueueWorker();
            _timer = new Timer(DefaultSpeed * Speed);
            _timer.Elapsed += (_, __) =>
            {
                _timer.Interval = DefaultSpeed * Speed;
                _threadWorker.Enqueue(() =>
                {
                    if (GoToNextFrame()) return;
                    _timer.Stop();
                    MainThreadInvoker.Enqueue(() => OnFinished?.Invoke());
                });
            };
        }

        #region IDataSourceOffline

        public ISourceTreeNode Data { get; }

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

        public string DisplayName { get; }
        public SettingsBag? Settings => null;
        public Texture2D? Logo { get; }

        public int AmountOfFrames => _frames.CurrentSize;

        public string Timestamp => $"{_frames.Current?.Timestamp ?? 0} ({Position})";

        public int Position
        {
            get => _frames.CurrentIndex;
            set => RewindAt(value);
        }

        public float Speed { get; set; } = 1;
        public bool IsPlaying { get; private set; }
        public event Action? OnPlayingStarted;
        public event Action? OnPaused;
        public event Action<int>? OnPositionChanged;
        public event Action<int>? OnAmountOfFramesChanged;
        public event Action<string>? OnTimestampChanged;

        public event Action? OnRewindStarted;
        public event Action? OnRewindFinished;
        public event Action? OnFinished;

        public void Play()
        {
            IsPlaying = true;
            OnPlayingStarted?.Invoke();
            _timer.Start();
        }

        public void Pause()
        {
            IsPlaying = false;
            _timer.Stop();
            OnPaused?.Invoke();
        }

        public void StopPlaying()
        {
            IsPlaying = false;
            _timer.Stop();
            OnPaused?.Invoke();
            _threadWorker.Enqueue(() =>
            {
                Data.Clear();
                _frames.SoftReset();
            });
        }

        public void PreviousKeyFrame()
        {
            OnRewindStarted?.Invoke();
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

                OnRewindFinished?.Invoke();
            });
        }

        public void NextKeyFrame()
        {
            OnRewindStarted?.Invoke();
            _threadWorker.Enqueue(() =>
            {
                do
                {
                    if (!GoToNextFrame()) break;
                } while (!(_frames.Current?.IsSpecial) ?? false);

                OnRewindFinished?.Invoke();
            });
        }

        public void PreviousFrame()
        {
            OnRewindStarted?.Invoke();
            _threadWorker.Enqueue(() =>
            {
                if (_frames.CurrentIndex == 0)
                {
                    _frames.Current?.Rewind();
                    _frames.SoftReset();
                    return;
                }

                GoToPreviousFrame();
                OnRewindFinished?.Invoke();
            });
        }

        public void NextFrame()
        {
            OnRewindStarted?.Invoke();
            _threadWorker.Enqueue(() =>
            {
                GoToNextFrame();
                OnRewindFinished?.Invoke();
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
        private const int DefaultSpeed = 2;

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
            if (!_frames.MovePrevious()) return false;

            (_containerTree.Image as FileImagePresenter)?.Present(_frames.Current);
            OnPositionChanged?.Invoke(Position);
            OnTimestampChanged?.Invoke(Timestamp);
            return true;
        }

        private bool GoToNextFrame()
        {
            if (!_frames.MoveNext()) return false;

            _frames.Current?.Show();
            (_containerTree.Image as FileImagePresenter)?.Present(_frames.Current);
            OnPositionChanged?.Invoke(Position);
            OnTimestampChanged?.Invoke(Timestamp);
            return true;
        }

        private void RewindAt(int pos)
        {
            if (pos < 0 || pos >= AmountOfFrames || pos == Position) return;

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