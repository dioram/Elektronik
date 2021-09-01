using System;
using Elektronik.Data;
using Elektronik.Data.Converters;
using Elektronik.Extensions;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.GrpcServices;
using Elektronik.Protobuf.Online.Presenters;
using Grpc.Core;
using UnityEngine;
using ILogger = Grpc.Core.Logging.ILogger;

namespace Elektronik.Protobuf.Online
{
    using GrpcServer = Server;

    public class ProtobufGrpcServer : IDataSourcePluginOnline
    {
        public ProtobufGrpcServer(OnlineSettingsBag settings, ICSConverter converter, ILogger? logger = null)
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
                new PointsMapManager(containerTree.Points, converter, logger),
                new ObservationsMapManager(containerTree.Observations, converter, logger),
                new TrackedObjsMapManager(containerTree.TrackedObjs, converter, logger),
                new LinesMapManager(containerTree.Lines, converter, logger),
                new InfinitePlanesMapManager(containerTree.InfinitePlanes, converter, logger)
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

        #endregion
    }
}