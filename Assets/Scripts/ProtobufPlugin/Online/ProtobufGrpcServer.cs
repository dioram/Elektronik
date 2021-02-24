using System;
using System.Collections.Generic;
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
        public string DisplayName => "Protobuf";
        public string Description => "Protobuf description";

        public IContainerTree[] Children { get; }

        private MapsManagerPb.MapsManagerPbBase[] _mapManagers;
        bool _serverStarted = false;
        
        public void SetActive(bool active)
        {
            foreach (var child in Children)
            {
                child.SetActive(active);
            }
        }

        public ICSConverter Converter { get; set; }
        
        public SettingsBag Settings { get; set; }

        public Type RequiredSettingsType => typeof(OnlineSettingsBag);

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
            var currentSettings = SettingsBag.GetCurrent<OnlineSettingsBag>();
            
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * currentSettings.Scale);
            
            var servicesChain = _mapManagers.Select(m => m as IChainable<MapsManagerPb.MapsManagerPbBase>).BuildChain();
            
            Debug.Log($"{currentSettings.IPAddress}:{currentSettings.Port}");
            
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
                            new ServerPort(currentSettings.IPAddress, currentSettings.Port, ServerCredentials.Insecure),
                    },
            };
            StartServer();
        }
        
        private void StartServer()
        {
            Stop();
            _server.Start();
            _serverStarted = true;
        }

        private void StopServer()
        {
        }

        public void Stop()
        {
            if (_serverStarted)
            {
                _server.ShutdownAsync().Wait();
                _serverStarted = false;
            }
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

        public void SetRenderers(ICloudRenderer[] renderers)
        {
            throw new System.NotImplementedException();
        }
        
        public GrpcServer _server;
        private OnlineSettingsBag _settings => (OnlineSettingsBag) Settings;
    }
}