using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Extensions;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.OnlineBuffered.GrpcServices;
using Elektronik.Protobuf.OnlineBuffered.Presenters;
using Elektronik.Settings.Bags;
using Grpc.Core;
using UnityEngine;
using ILogger = Grpc.Core.Logging.ILogger;

namespace Elektronik.Protobuf.OnlineBuffered
{
    using GrpcServer = Server;

    public class ProtobufOnlinePlayer : IDataSourcePlugin
    {
        public ProtobufOnlinePlayer(string displayName, Texture2D? logo, OnlineSettingsBag settings,
                                    ICSConverter converter, ILogger? logger = null)
        {
            var containerTree = new ProtobufContainerTree("Protobuf", new RawImagePresenter("Camera"));
            Data = containerTree;
            DisplayName = displayName;
            Logo = logo;
            _logger = logger ?? new UnityLogger();
            _port = settings.ListeningPort;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            GrpcEnvironment.SetLogger(_logger);
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);

            _services = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(_buffer, containerTree.Points, converter, _logger),
                new ObservationsMapManager(_buffer, containerTree.Observations, converter, _logger),
                new TrackedObjsMapManager(_buffer, containerTree.TrackedObjs, converter, _logger),
                new LinesMapManager(_buffer, containerTree.Lines, converter, _logger),
                new InfinitePlanesMapManager(_buffer, containerTree.InfinitePlanes, converter, _logger)
            }.BuildChain();

            _buffer.FramesAmountChanged += i => OnAmountOfFramesChanged?.Invoke(i);

            containerTree.DisplayName = $"From gRPC at port {settings.ListeningPort}";
            StartServer();
        }

        #region IDataSourcePlugin

        public void Play()
        {
            _playingCancellation = new CancellationTokenSource();
            _playTask = Task.Run(() => Play(_playingCancellation.Token));
        }

        public void Pause()
        {
            _playingCancellation?.Cancel();
            _playTask?.Wait();
            OnPaused?.Invoke();
        }

        public void StopPlaying()
        {
            _playingCancellation?.Cancel();
            _playTask?.Wait();
            OnPaused?.Invoke();

            _server.ShutdownAsync().Wait();
            _serverStarted = false;

            _buffer.Reset();
            Data.Clear();

            StartServer();
        }

        public void PreviousKeyFrame()
        {
            _playingCancellation?.Cancel();
            _playTask?.Wait();
            if (_buffer.CurrentSize == 0) return;
            Task.Run(() =>
            {
                GoToPreviousFrame();
                while (_buffer.Current!.IsKeyFrame && GoToPreviousFrame())
                {
                }
            });
        }

        public void NextKeyFrame()
        {
            _playingCancellation?.Cancel();
            _playTask?.Wait();
            if (_buffer.CurrentSize == 0) return;
            Task.Run(() =>
            {
                GoToNextFrame();
                while (_buffer.Current!.IsKeyFrame && GoToNextFrame())
                {
                }
            });
        }

        public void PreviousFrame()
        {
            _playingCancellation?.Cancel();
            _playTask?.Wait();
            GoToPreviousFrame();
        }

        public void NextFrame()
        {
            _playingCancellation?.Cancel();
            _playTask?.Wait();
            GoToNextFrame();
        }

        public ISourceTreeNode Data { get; }
        public int AmountOfFrames => _buffer.CurrentSize;
        public string Timestamp => $"{_buffer.CurrentTimeStamp:HH:mm:ss.ff}";

        public int Position
        {
            get => _buffer.CurrentIndex;
            set => Rewind(value);
        }

        public float Speed { get; set; }
        public bool IsPlaying { get; private set; }
        public event Action? OnPlayingStarted;
        public event Action? OnPaused;
        public event Action<int>? OnPositionChanged;
        public event Action<int>? OnAmountOfFramesChanged;
        public event Action<string>? OnTimestampChanged;

        public event Action? OnRewindStarted;
        public event Action? OnRewindFinished;
        public event Action? OnFinished;

        public void Dispose()
        {
            Data.Clear();
            if (!_serverStarted) return;
            _server.ShutdownAsync().Wait();
            _serverStarted = false;
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        public string DisplayName { get; }
        public SettingsBag? Settings => null;

        public Texture2D? Logo { get; }

        #endregion

        #region Private definitions

        private GrpcServer _server;
        private readonly OnlineFrameBuffer _buffer = new();
        private bool _serverStarted = false;
        private readonly int _port;
        private readonly ILogger _logger;
        private readonly MapsManagerPb.MapsManagerPbBase? _services;
        private CancellationTokenSource? _playingCancellation;
        private Task? _playTask;

        private void Play(CancellationToken token)
        {
            IsPlaying = true;
            OnPlayingStarted?.Invoke();
            while (true)
            {
                if (token.IsCancellationRequested) break;
                try
                {
                    var timer = Stopwatch.StartNew();
                    if (GoToNextFrame())
                    {
                        timer.Stop();
                        if (token.IsCancellationRequested) break;
                        var delay = _buffer.Current!.ToNext.TotalMilliseconds * Speed - timer.Elapsed.TotalMilliseconds;
                        if (delay > 0) Thread.Sleep((int)delay);
                    }
                    else
                    {
                        timer.Stop();
                        Thread.Sleep(10);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "");
                }
            }

            IsPlaying = false;
        }

        private void StartServer()
        {
            _server = new GrpcServer
            {
                Services =
                {
                    MapsManagerPb.BindService(_services),
                    SceneManagerPb.BindService(new SceneManager(Data, _logger)),
                    ImageManagerPb.BindService(
                        new ImageManager(((ProtobufContainerTree)Data).Image as RawImagePresenter, _logger))
                },
                Ports = { new ServerPort("0.0.0.0", _port, ServerCredentials.Insecure) },
            };
            _server.Start();
            _serverStarted = true;
        }

        private void Rewind(int newPos)
        {
            if (newPos == Position) return;
            OnRewindStarted?.Invoke();

            if (_playingCancellation?.IsCancellationRequested ?? true)
            {
                Task.Run(() => DoRewind(newPos, false));
            }
            else
            {
                _playingCancellation?.Cancel();
                _playTask?.Wait();
                Task.Run(() => DoRewind(newPos, true));
            }
        }

        private void DoRewind(int pos, bool continuePlaying)
        {
            try
            {
                while (Position != pos)
                {
                    if (Position < pos) GoToNextFrame();
                    else GoToPreviousFrame();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "");
            }

            OnRewindFinished?.Invoke();
            if (continuePlaying) Play();
        }

        private bool GoToNextFrame()
        {
            if (!_buffer.MoveNext()) return false;

            _buffer.Current!.Command.Execute();
            OnPositionChanged?.Invoke(Position);
            OnTimestampChanged?.Invoke(Timestamp);
            return true;
        }

        private bool GoToPreviousFrame()
        {
            _buffer.Current!.Command.UnExecute();
            if (!_buffer.MovePrevious()) return false;

            OnPositionChanged?.Invoke(Position);
            OnTimestampChanged?.Invoke(Timestamp);
            return true;
        }

        #endregion
    }
}