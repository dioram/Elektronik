using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Elektronik.Common
{
    [RequireComponent(typeof(VRTK_SDKManager))]
    public class VRTKManagerInitializer : MonoBehaviour
    {
        [HideInInspector]
        public VRTK_SDKSetup Setup { get; set; }

        private void OnEnable()
        {
            GetComponent<VRTK_SDKManager>().TryLoadSDKSetup(0, false, Setup);
        }

        private void OnDisable()
        {
            GetComponent<VRTK_SDKManager>().UnloadSDKSetup(true);
        }
    }
}
