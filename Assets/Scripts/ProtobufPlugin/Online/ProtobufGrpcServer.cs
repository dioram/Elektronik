using System;
using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using Elektronik.Common.Settings;
using Elektronik.PluginsSystem;
using Elektronik.ProtobufPlugin.Online.GrpcServices;
using Grpc.Core;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Online
{
    using GrpcServer = Grpc.Core.Server;
    
    public class ProtobufGrpcServer : IDataSourceOnline
    {
        public ProtobufGrpcServer()
        {
            _containerTree = new ProtobufContainerTree("Protobuf");
        }
        
        #region IDataSourceOnline implementation

        public string DisplayName => "Protobuf";
        
        public string Description => "Protobuf description";
        
        public ICSConverter Converter { get; set; }

        public IContainerTree Data => _containerTree;

        public SettingsBag Settings => _onlineSettings;

        public void Start()
        {
            if (!(Settings is OnlineSettingsBag))
            {
                throw new InvalidCastException($"Wrong type of settings for {DisplayName}-{nameof(ProtobufGrpcServer)}");
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
                            //ImageManagerPb.BindService(new ImageManager(imageRenderTarget))
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
        }

        public void Update()
        {
            // Do nothing
        }
        
        #endregion

        #region Private definitions

        private ProtobufContainerTree _containerTree;
        
        private bool _serverStarted = false;

        private void StartServer()
        {
            Stop();
            _server.Start();
            _serverStarted = true;
        }

        private GrpcServer _server;
        
        private readonly OnlineSettingsBag _onlineSettings = new OnlineSettingsBag();

        #endregion
    }
}