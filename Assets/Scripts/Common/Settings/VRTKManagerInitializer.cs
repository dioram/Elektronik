using UnityEngine;
using VRTK;

namespace Elektronik.Common.Settings
{
    [RequireComponent(typeof(VRTK_SDKManager))]
    public class VRTKManagerInitializer : MonoBehaviour
    {
        [HideInInspector]
        public VRTK_SDKSetup Setup { get; set; }

        private void OnEnable()
        {
            var manager = GetComponent<VRTK_SDKManager>();
            manager.TryLoadSDKSetup(0, false, Setup);
        }

        private void OnDisable()
        {
            var manager = GetComponent<VRTK_SDKManager>();
            manager.UnloadSDKSetup();
        }
    }
}
