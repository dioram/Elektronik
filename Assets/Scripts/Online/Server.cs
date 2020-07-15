using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elektronik.Online.GrpcServices;
using Grpc.Core;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using Elektronik.Common.Maps;
using Elektronik.Common;
using Elektronik.Online.Settings;
using Elektronik.Common.Settings;
using UnityEngine.UI;
using System.Net;
using System;

namespace Elektronik.Online
{
    using GrpcServer = Grpc.Core.Server;

    public partial class Server : MonoBehaviour
    {
        public Text status;
        public SlamMap slamMaps;
        GrpcServer m_server;
        bool m_serverStarted = false;

        // Start is called before the first frame update
        void Start()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            GrpcEnvironment.SetLogger(new UnityLogger());
            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(slamMaps.Points),
                new ObservationsMapManager(slamMaps.Observations),
                new TrackedObjsMapManager(slamMaps.TrackedObjsGO, slamMaps.TrackedObjs),
                new LinesMapManager(slamMaps.Lines),
            }.BuildChain();

            Debug.Log($"{SettingsBag.Current[SettingName.IPAddress].As<IPAddress>()}:{SettingsBag.Current[SettingName.Port].As<int>()}");

            m_server = new GrpcServer()
            {
                Services = 
                { 
                    MapsManagerPb.BindService(servicesChain), 
                    SceneManagerPb.BindService(new SceneManager(slamMaps)),
                },
                Ports =
                {
                    new ServerPort(
                        SettingsBag.Current[SettingName.IPAddress].As<IPAddress>().ToString(),
                        SettingsBag.Current[SettingName.Port].As<int>(), 
                        ServerCredentials.Insecure),
                },
            };
            StartServer();
        }

        private void StartServer()
        {
            StopServer();
            m_server.Start();
            status.color = Color.green;
            status.text = "gRPC server started";
            m_serverStarted = true;
        }

        private void StopServer()
        {
            if (m_serverStarted)
            {
                m_server.ShutdownAsync().Wait();
                status.color = Color.red;
                status.text = "gRPC server stopped";
                m_serverStarted = false;
            }
        }

        private void OnDestroy()
        {
            StopServer();
        }
    }
}