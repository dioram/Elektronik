using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRTK;

namespace Elektronik.Online
{
    class VRHacker_New : MonoBehaviour
    {
        public Helmet helmet;

        private void Start()
        {
            UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        }

        private void Update()
        {
            Transform headset = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
            if (headset != null)
            {
                headset.rotation = helmet.transform.rotation;
                headset.position = helmet.transform.position + new Vector3(0, 1, 0);
            }
        }
    }
}
