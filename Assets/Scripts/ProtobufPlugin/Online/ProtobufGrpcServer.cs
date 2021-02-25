using System;
using System.Linq;
using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using Elektronik.Common.Settings;
using Elektronik.Online.GrpcServices;
using Elektronik.Online.Settings;
using Elektronik.PluginsSystem;
using Grpc.Core;
using ProtobufPlugin.Online;
using UnityEngine;

namespace Elektronik.ProtobufPlugin.Online
{
    using GrpcServer = Grpc.Core.Server;
    
    public class ProtobufGrpcServer : IDataSourceOnline
    {
        #region IDataSourceOnline implementation

        public string DisplayName => "Protobuf";
        
        public string Description => "Protobuf description";
        
        public IContainerTree[] Children { get; }
        
        public ICSConverter Converter { get; set; }
        
        public SettingsBag Settings { get; set; }

        public Type RequiredSettingsType => typeof(OnlineSettingsBag);
        
        public void SetActive(bool active)
        {
            foreach (var child in Children)
            {
                child.SetActive(active);
            }
        }
        
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
            
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * OnlineSettings.Scale);
            
            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                    // new PointsMapManager(slamMaps.Points, Converter),
                    // new ObservationsMapManager(slamMaps.Observations, Converter),
                    // new TrackedObjsMapManager(slamMaps.TrackedObjsGO, slamMaps.TrackedObjs, Converter),
                    // new LinesMapManager(slamMaps.Lines, Converter),
            }.BuildChain();
            
            Debug.Log($"{OnlineSettings.IPAddress}:{OnlineSettings.Port}");
            
            _server = new GrpcServer()
            {
                    Services = 
                    { 
                            MapsManagerPb.BindService(servicesChain), 
                            SceneManagerPb.BindService(new SceneManager(Children)),
                            //ImageManagerPb.BindService(new ImageManager(imageRenderTarget))
                    },
                    Ports =
                    {
                            new ServerPort(OnlineSettings.IPAddress, OnlineSettings.Port, ServerCredentials.Insecure),
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

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        // public void SetRenderers(ICloudRenderer[] renderers)
        // {
        //     foreach (var child in Children)
        //     {
        //         child.SetRenderers(renderers);
        //     }
        // }
        
        #endregion

        #region Private definitions

        private MapsManagerPb.MapsManagerPbBase[] _mapManagers;
        
        private bool _serverStarted = false;

        private void StartServer()
        {
            Stop();
            _server.Start();
            _serverStarted = true;
        }

        private GrpcServer _server;
        
        private OnlineSettingsBag OnlineSettings => (OnlineSettingsBag) Settings;

        #endregion
    }
}