using UnityEngine;
using Elektronik.Online.GrpcServices;
using Grpc.Core;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Extensions;
using Elektronik.Common;
using Elektronik.Online.Settings;
using UnityEngine.UI;
using System;
using System.Linq;
using Elektronik.Common.Cameras;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Settings;

namespace Elektronik.Online
{
    using GrpcServer = Grpc.Core.Server;

    public partial class Server : MonoBehaviour
    {
        public Text status;
        public CSConverter converter;
        public CameraImageRenderer imageRenderTarget;
        public GameObject Containers;

        public MapsManagerPb.MapsManagerPbBase[] MapManagers;

        public void ClearAll()
        {
            foreach (var container in Containers.GetComponentsInChildren<IClearable>())
            {
                container.Clear();
            }
        }
        
        bool _serverStarted = false;
        GrpcServer _server;

        // Start is called before the first frame update
        void Start()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            GrpcEnvironment.SetLogger(new UnityLogger());
            var currentSettings = SettingsBag.GetCurrent<OnlineSettingsBag>();

            converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * currentSettings.Scale);

            var servicesChain = MapManagers.Select(m => m as IChainable<MapsManagerPb.MapsManagerPbBase>).BuildChain();

            Debug.Log($"{currentSettings.IPAddress}:{currentSettings.Port}");

            _server = new GrpcServer()
            {
                Services = 
                { 
                    MapsManagerPb.BindService(servicesChain), 
                    //SceneManagerPb.BindService(new SceneManager(Containers)),
                    ImageManagerPb.BindService(new ImageManager(imageRenderTarget))
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
            StopServer();
            _server.Start();
            status.color = Color.green;
            status.text = "gRPC server started";
            _serverStarted = true;
        }

        private void StopServer()
        {
            if (_serverStarted)
            {
                _server.ShutdownAsync().Wait();
                status.color = Color.red;
                status.text = "gRPC server stopped";
                _serverStarted = false;
            }
        }

        private void OnDestroy()
        {
            StopServer();
        }
    }
}