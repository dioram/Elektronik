using System;
using Elektronik.Extensions;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.GrpcServices;
using Elektronik.Protobuf.Online.Presenters;
using Elektronik.Settings.Bags;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.Protobuf.Online
{
    using GrpcServer = Server;

    public class ProtobufGrpcServer : DataSourcePluginBase<PortScaleSettingsBag>, IDataSourcePluginOnline
    {
        public ProtobufGrpcServer()
        {
            _containerTree = new ProtobufContainerTree("Protobuf", new RawImagePresenter("Camera"));
            Data = _containerTree;
        }

        #region IDataSourceOnline implementation

        public override string DisplayName => "Protobuf";

        public override string Description => "This plugin plays data coming through " +
                "<#7f7fe5><u><link=\"https://grpc.io/\">gRPC</link></u></color> with " +
                "<#7f7fe5><u><link=\"https://developers.google.com/protocol-buffers/\">" +
                "Protocol buffers</link></u></color>. " +
                "You can find documentation for data package format " +
                "<#7f7fe5><u><link=\"https://github.com/dioram/Elektronik-Tools-2.0/blob/master/docs/Protobuf-EN.md\">" +
                "here</link></u></color>. Also you can see *.proto files in <ElektronikDir>/Plugins/Protobuf/data.";

        public override void Start()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            GrpcEnvironment.SetLogger(new UnityLogger());

            Converter?.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);

            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(_containerTree.Points, Converter),
                new ObservationsMapManager(_containerTree.Observations, Converter),
                new TrackedObjsMapManager(_containerTree.TrackedObjs, Converter),
                new LinesMapManager(_containerTree.Lines, Converter),
                new InfinitePlanesMapManager(_containerTree.InfinitePlanes, Converter)
            }.BuildChain();

            _containerTree.DisplayName = $"From gRPC at port {TypedSettings.Port}";
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log($"{TypedSettings.IPAddress}:{TypedSettings.Port}");
#endif

            _server = new GrpcServer()
            {
                Services =
                {
                    MapsManagerPb.BindService(servicesChain),
                    SceneManagerPb.BindService(new SceneManager(_containerTree)),
                    ImageManagerPb.BindService(new ImageManager((RawImagePresenter) _containerTree.Image))
                },
                Ports =
                {
                    new ServerPort("0.0.0.0", TypedSettings.Port, ServerCredentials.Insecure),
                },
            };
            StartServer();
        }

        public override void Stop()
        {
            if (!_serverStarted) return;
            _server.ShutdownAsync().Wait();
            _serverStarted = false;
            Data.Clear();
        }

        public override void Update(float delta)
        {
            // Do nothing
        }

        #endregion

        #region Private definitions

        private readonly ProtobufContainerTree _containerTree;

        private bool _serverStarted = false;

        private void StartServer()
        {
            Stop();
            _server.Start();
            _serverStarted = true;
        }

        private GrpcServer _server;

        #endregion
    }
}