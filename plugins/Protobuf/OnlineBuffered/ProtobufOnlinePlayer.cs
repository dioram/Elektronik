using System;
using Elektronik.Commands;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Extensions;
using Elektronik.Offline;
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
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            logger ??= new UnityLogger();
            GrpcEnvironment.SetLogger(logger);
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);

            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(_buffer, containerTree.Points, converter, logger),
                new ObservationsMapManager(_buffer, containerTree.Observations, converter, logger),
                new TrackedObjsMapManager(_buffer, containerTree.TrackedObjs, converter, logger),
                new LinesMapManager(_buffer, containerTree.Lines, converter, logger),
                new InfinitePlanesMapManager(_buffer, containerTree.InfinitePlanes, converter, logger)
            }.BuildChain();

            containerTree.DisplayName = $"From gRPC at port {settings.ListeningPort}";

            _server = new GrpcServer
            {
                Services =
                {
                    MapsManagerPb.BindService(servicesChain),
                    SceneManagerPb.BindService(new SceneManager(containerTree, logger)),
                    ImageManagerPb.BindService(new ImageManager(containerTree.Image as RawImagePresenter, logger))
                },
                Ports = { new ServerPort("0.0.0.0", settings.ListeningPort, ServerCredentials.Insecure) }
            };
            _buffer.FramesAmountChanged += _ =>
            {
                if (_buffer.MoveNext()) _buffer.Current!.Execute();
            };
            _server.Start();
            _serverStarted = true;
        }

        #region IDataSourcePlugin

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void StopPlaying()
        {
            throw new NotImplementedException();
        }

        public void PreviousKeyFrame()
        {
            throw new NotImplementedException();
        }

        public void NextKeyFrame()
        {
            throw new NotImplementedException();
        }

        public void PreviousFrame()
        {
            throw new NotImplementedException();
        }

        public void NextFrame()
        {
            throw new NotImplementedException();
        }

        public ISourceTree Data { get; }
        public int AmountOfFrames { get; }
        public string CurrentTimestamp { get; }
        public int CurrentPosition { get; set; }
        public event Action<bool>? Rewind;
        public event Action? Finished;

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

        private bool _serverStarted = false;

        private readonly GrpcServer _server;
        private readonly UpdatableFramesCollection<ICommand> _buffer = new();

        #endregion
    }
}