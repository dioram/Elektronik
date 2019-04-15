using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Elektronik.Common
{
    [RequireComponent(typeof(Camera))]
    public class DisableAutoXR : MonoBehaviour
    {
        public bool @default;
        private bool m_current;

        void Start()
        {
            DisableXR(@default);
        }

        public void DisableXR(bool disable)
        {
            m_current = disable;
            XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), disable);
        }

        void OnEnable()
        {
            DisableXR(m_current);
        }
    }
}