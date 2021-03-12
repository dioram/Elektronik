using System;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Extensions;
using Elektronik.PluginsSystem;
using Elektronik.Presenters;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.GrpcServices;
using Elektronik.Protobuf.Online.Presenters;
using Elektronik.Settings;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.Protobuf.Online
{
    using GrpcServer = Grpc.Core.Server;

    public class ProtobufGrpcServer : IDataSourceOnline
    {
        public ProtobufGrpcServer()
        {
            _containerTree = new ProtobufContainerTree("Protobuf");
            _imagePresenter = new ImagePresenter();
            PresentersChain = new DataPresenter[]
            {
                _imagePresenter
            }.BuildChain();
            
            var sh = new OnlineSettingsHistory();
            SettingsHistory = sh;
            if (sh.Recent.Count > 0) _onlineSettings = (OnlineSettingsBag) sh.Recent[0];
            else _onlineSettings = new OnlineSettingsBag();
        }

        #region IDataSourceOnline implementation

        public string DisplayName => "Protobuf";

        public string Description => "Protocol buffers through gRPC.";

        public ICSConverter Converter { get; set; }

        public IContainerTree Data => _containerTree;
        public DataPresenter PresentersChain { get; private set; }

        public SettingsBag Settings
        {
            get => _onlineSettings;
            set => _onlineSettings = (OnlineSettingsBag)value;
        }

        public ISettingsHistory SettingsHistory { get; }

        public void Start()
        {
            if (!(Settings is OnlineSettingsBag))
            {
                throw new InvalidCastException(
                    $"Wrong type of settings for {DisplayName}-{nameof(ProtobufGrpcServer)}");
            }

            if (Converter == null)
            {
                throw new NullReferenceException(
                    $"Converter is not set for {DisplayName}-{nameof(ProtobufGrpcServer)}");
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            GrpcEnvironment.SetLogger(new UnityLogger());

            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * _onlineSettings.Scale);

            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(_containerTree.Points, Converter),
                new ObservationsMapManager(_containerTree.Observations, Converter),
                new TrackedObjsMapManager(_containerTree.TrackedObjs, Converter),
                new LinesMapManager(_containerTree.Lines, Converter),
                new InfinitePlanesMapManager(_containerTree.InfinitePlanes, Converter)
            }.BuildChain();

            _containerTree.DisplayName = $"From gRPC {_onlineSettings.IPAddress}:{_onlineSettings.Port}";
            Debug.Log($"{_onlineSettings.IPAddress}:{_onlineSettings.Port}");

            _server = new GrpcServer()
            {
                Services =
                {
                    MapsManagerPb.BindService(servicesChain),
                    SceneManagerPb.BindService(new SceneManager(_containerTree)),
                    ImageManagerPb.BindService(new ImageManager(_imagePresenter))
                },
                Ports =
                {
                    new ServerPort(_onlineSettings.IPAddress, _onlineSettings.Port, ServerCredentials.Insecure),
                },
            };
            StartServer();
        }

        public void Stop()
        {
            if (!_serverStarted) return;
            _server.ShutdownAsync().Wait();
            _serverStarted = false;
            Data.Clear();
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        #endregion

        #region Private definitions

        private readonly ProtobufContainerTree _containerTree;
        private readonly ImagePresenter _imagePresenter;

        private bool _serverStarted = false;

        private void StartServer()
        {
            Stop();
            _server.Start();
            _serverStarted = true;
        }

        private GrpcServer _server;

        private OnlineSettingsBag _onlineSettings;

        #endregion
    }
}