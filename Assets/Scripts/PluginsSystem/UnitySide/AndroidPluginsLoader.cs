using System.Collections.Generic;
using Elektronik.Protobuf.Offline;
using Elektronik.Protobuf.Online;
using Elektronik.Protobuf.Recorders;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

namespace Elektronik.PluginsSystem.UnitySide
{
    public class AndroidPluginsLoader : MonoBehaviour
    {
        public static readonly List<IElektronikPluginsFactory> Factories = new List<IElektronikPluginsFactory>();

        public void Start()
        {
#if PLATFORM_ANDROID
            Factories.Add(new ProtobufFilePlayerFactory());
            Factories.Add(new ProtobufOnlinePlayerFactory());
            Factories.Add(new ProtobufRecorderFactory());
            Factories.Add(new ProtobufRetranslatorFactory());
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
#endif
            OnEverythingLoaded();
        }
        
        private void OnEverythingLoaded()
        {
            SceneManager.LoadScene("Main");
        }
    }
}