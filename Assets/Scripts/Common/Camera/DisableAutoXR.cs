using UnityEngine;
using UnityEngine.XR;

namespace Elektronik.Common.Cameras
{
    [RequireComponent(typeof(UnityEngine.Camera))]
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
            XRDevice.DisableAutoXRCameraTracking(GetComponent<UnityEngine.Camera>(), disable);
        }

        void OnEnable()
        {
            DisableXR(m_current);
        }
    }
}