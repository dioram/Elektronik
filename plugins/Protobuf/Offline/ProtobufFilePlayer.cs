using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Elektronik.Extensions;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Parsers;
using Elektronik.Protobuf.Offline.Presenters;
using Elektronik.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayer : DataSourcePluginBase<OfflineSettingsBag>, IDataSourcePluginOffline
    {
        public ProtobufFilePlayer()
        {
            _containerTree = new ProtobufContainerTree("Protobuf",
                                                       new FileImagePresenter("Camera", TypedSettings.ImagePath),
                                                       new SlamDataInfoPresenter("Special info"));
            Data = _containerTree;
            _parsersChain = new DataParser<PacketPb>[]
            {
                new ObjectsParser(_containerTree.InfinitePlanes,
                                  _containerTree.Points,
                                  _containerTree.Observations,
                                  TypedSettings.ImagePath),
                new TrackedObjectsParser(_containerTree.TrackedObjs),
                new InfoParser(_containerTree.SpecialInfo),
            }.BuildChain();
        }

        #region IDataSourceOffline

        public override string DisplayName => "Protobuf";

        public override string Description => "This plugin reads " +
                "<#7f7fe5><u><link=\"https://developers.google.com/protocol-buffers/\">protobuf</link></u></color>" +
                " packages from file. You can find documentation for data package format " +
                "<#7f7fe5><u><link=\"https://github.com/dioram/Elektronik-Tools-2.0/blob/master/docs/Protobuf-EN.md\">" +
                "here</link></u></color>. Also you can see *.proto files in <ElektronikDir>/Plugins/Protobuf/data.";

        public override void Start()
        {
            _containerTree.DisplayName = $"Protobuf: {Path.GetFileName(TypedSettings.FilePath)}";
            _input = File.OpenRead(TypedSettings.FilePath!);
            Converter?.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);
            _parsersChain.SetConverter(Converter);

            _frames = new FramesCollection<Frame>(ReadCommands, TryGetSize());
            _threadWorker = new ThreadQueueWorker();
            _timer = new Timer(UpdateDeltaMS);
            _timer.Elapsed += (_, __) =>
            {
                _threadWorker.Enqueue(() =>
                {
                    if (NextFrame()) return;
                    _timer?.Stop();
                    MainThreadInvoker.Enqueue(() => Finished?.Invoke());
                });
            };
        }

        public override void Stop()
        {
            Data.Clear();
            _input.Dispose();
            _threadWorker.Dispose();
        }

        public override void Update(float delta)
        {
            // Do nothing
        }

        public void SetFileName(string filename)
        {
            TypedSettings.FilePath = filename;
        }

        public int AmountOfFrames => _frames?.CurrentSize ?? 0;

        public string CurrentTimestamp => $"{_frames?.Current?.Timestamp ?? 0} ({CurrentPosition})";
        public string[] SupportedExtensions { get; } = {".dat"};

        public int CurrentPosition
        {
            get => _frames?.CurrentIndex ?? 0;
            set => RewindAt(value);
        }

        public event Action<bool> Rewind;

        public void Play()
        {
            _timer?.Start();
        }

        public void Pause()
        {
            _timer?.Stop();
        }

        public void StopPlaying()
        {
            _timer?.Stop();
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
                if (_frames?.CurrentIndex == 0)
                {
                    _frames?.Current?.Rewind();
                    _frames?.SoftReset();
                    return;
                }
                do
                {
                    if (!PreviousFrame()) break;
                } while (!(_frames?.Current?.IsSpecial) ?? false);

            });
        }

        public void NextKeyFrame()
        {
            _threadWorker.Enqueue(() =>
            {
                do
                {
                    if (!NextFrame()) break;
                } while (!(_frames?.Current?.IsSpecial) ?? false);
            });
        }

        public event Action Finished;

        #endregion

        #region Private definitions

        private const int UpdateDeltaMS = 2;

        private readonly ProtobufContainerTree _containerTree;
        private FileStream _input;
        private FramesCollection<Frame> _frames;
        private readonly DataParser<PacketPb> _parsersChain;
        private ThreadQueueWorker _threadWorker;
        [CanBeNull] private Timer _timer;

        private IEnumerator<Frame> ReadCommands(int size)
        {
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
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

        private bool PreviousFrame()
        {
            _frames?.Current?.Rewind();
            if (_frames?.MovePrevious() ?? false)
            {
                (_containerTree.Image as FileImagePresenter)?.Present(_frames?.Current);
                return true;
            }

            return false;
        }

        private bool NextFrame()
        {
            if (_frames.MoveNext())
            {
                _frames?.Current?.Show();
                (_containerTree.Image as FileImagePresenter)?.Present(_frames?.Current);
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