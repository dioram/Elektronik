using UnityEngine;
using Elektronik.Online.GrpcServices;
using Grpc.Core;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using Elektronik.Common;
using Elektronik.Online.Settings;
using Elektronik.Common.Settings;
using UnityEngine.UI;
using System;
using Elektronik.Common.Cameras;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Online
{
    using GrpcServer = Grpc.Core.Server;

    public partial class Server : MonoBehaviour
    {
        public Text status;
        public CSConverter converter;
        public CameraImageRenderer imageRenderTarget;
        public SlamInfinitePlanesContainer InfinitePlanesContainer;
        public ConnectableObjectsContainer<SlamPoint> ConnectablePointsContainer;
        public ConnectableObjectsContainer<SlamObservation> ConnectableObservationsContainer;
        public ConnectableTrackedObjsContainer ConnectableTrackedObjsContainer;
        public SlamLinesContainer LinesContainer;
        public GameObject Containers;

        public void ClearAll()
        {
            foreach (var container in Containers.GetComponentsInChildren<IClearable>())
            {
                container.Clear();
            }
        }
        
        GrpcServer m_server;
        bool m_serverStarted = false;

        // Start is called before the first frame update
        void Start()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            GrpcEnvironment.SetLogger(new UnityLogger());

            converter.SetInitTRS(Vector3.zero, Quaternion.identity, 
                Vector3.one * SettingsBag.Current[SettingName.Scale].As<float>());

            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(ConnectablePointsContainer, converter),
                new ObservationsMapManager(ConnectableObservationsContainer, converter),
                new TrackedObjsMapManager(ConnectableTrackedObjsContainer.TrackedObjsContainer, ConnectableTrackedObjsContainer, converter),
                new LinesMapManager(LinesContainer, converter),
                new InfinitePlanesMapManager(InfinitePlanesContainer, converter)
            }.BuildChain();

            Debug.Log($"{SettingsBag.Current[SettingName.IPAddress].As<string>()}:{SettingsBag.Current[SettingName.Port].As<int>()}");

            m_server = new GrpcServer()
            {
                Services = 
                { 
                    MapsManagerPb.BindService(servicesChain), 
                    SceneManagerPb.BindService(new SceneManager(Containers)),
                    ImageManagerPb.BindService(new ImageManager(imageRenderTarget))
                },
                Ports =
                {
                    new ServerPort(
                        SettingsBag.Current[SettingName.IPAddress].As<string>(),
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