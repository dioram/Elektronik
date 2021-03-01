using System;
using Elektronik.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;
using Elektronik.Common.Data.Pb;
using System.IO;
using System.Threading.Tasks;
using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Presenters;
using Elektronik.Common.Settings;
using Elektronik.ProtobufPlugin.Offline.Parsers;
using Elektronik.PluginsSystem;
using Elektronik.ProtobufPlugin.Offline.Presenters;

namespace Elektronik.ProtobufPlugin.Offline
{
    public class ProtobufFilePlayer : IDataSourceOffline
    {
        public const float Timeout = 0.5f;
        public static readonly OfflineSettingsBag OfflineSettings = new OfflineSettingsBag();

        public ProtobufFilePlayer()
        {
            _containerTree = new ProtobufContainerTree("Protobuf");
            _parsersChain = new PackageParser[]
            {
                new ObjectsParser(_containerTree.InfinitePlanes,
                                  _containerTree.Points,
                                  _containerTree.Observations),
                new TrackedObjectsParser(_containerTree.TrackedObjs),
                new InfoParser(),
            }.BuildChain();

            PresentersChain = new DataPresenter[]
            {
                new ImagePresenter(OfflineSettings),
                new SlamDataInfoPresenter(_containerTree.Points, _containerTree.Observations),
            }.BuildChain();
        }

        #region IDataSourceOffline

        public string DisplayName => "Protobuf";

        public string Description => "Protobuf description";

        public SettingsBag Settings => OfflineSettings;

        public void Start()
        {
            _containerTree.DisplayName = $"Protobuf: {Path.GetFileName(OfflineSettings.FilePath)}";
            _input = File.OpenRead(OfflineSettings.FilePath!);
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * OfflineSettings.Scale);
            _parsersChain.SetConverter(Converter);
            _frames = new FramesCollection<Frame>(ReadCommands());
            _threadWorker = new ThreadWorker();
        }

        public void Stop()
        {
            _input.Dispose();
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
                    MainThreadInvoker.Instance.Enqueue(() => Finished?.Invoke());
                    _playing = false;
                }));
                _timeout = Timeout;
            }
        }

        public ICSConverter Converter { get; set; }

        public IContainerTree Data => _containerTree;

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

        public DataPresenter PresentersChain { get; }

        #endregion

        #region Private definitions

        private readonly ProtobufContainerTree _containerTree;
        private FileStream _input;
        private FramesCollection<Frame> _frames;
        private readonly PackageParser _parsersChain;
        private bool _playing = false;
        private float _timeout = 0;
        private ThreadWorker _threadWorker;

        private IEnumerator<Frame> ReadCommands()
        {
            while (_input.Position != _input.Length)
            {
                var packet = PacketPb.Parser.ParseDelimitedFrom(_input);
                yield return Frame.ParsePacket(packet, _parsersChain);
            }

            _input.Dispose();
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