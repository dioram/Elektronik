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
using Grpc.Core;
using UnityEngine;
using ILogger = Grpc.Core.Logging.ILogger;

namespace Elektronik.Protobuf.OnlineBuffered
{
    using GrpcServer = Server;

    public class ProtobufOnlinePlayer : IDataSourcePluginOnline
    {
        public ProtobufOnlinePlayer(OnlineSettingsBag settings, ICSConverter converter, ILogger? logger = null)
        {
            var containerTree = new ProtobufContainerTree("Protobuf", new RawImagePresenter("Camera"));
            Data = containerTree;
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

            containerTree.DisplayName = $"From gRPC at port {settings.Port}";
            
            _server = new GrpcServer
            {
                Services =
                {
                    MapsManagerPb.BindService(servicesChain),
                    SceneManagerPb.BindService(new SceneManager(containerTree, logger)),
                    ImageManagerPb.BindService(new ImageManager(containerTree.Image as RawImagePresenter, logger))
                },
                Ports = { new ServerPort("0.0.0.0", settings.Port, ServerCredentials.Insecure) }
            };
            _buffer.FramesAmountChanged += _ =>
            {
                if (_buffer.MoveNext()) _buffer.Current!.Execute();
            };
            _server.Start();
            _serverStarted = true;
        }

        #region IDataSourceOnline implementation

        public ISourceTree Data { get; }

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

        #endregion

        #region Private definitions

        private bool _serverStarted = false;

        private readonly GrpcServer _server;
        private readonly UpdatableFramesCollection<ICommand> _buffer = new ();

        #endregion
    }
}