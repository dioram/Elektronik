using UnityEngine;
using UnityEngine.XR;

namespace Elektronik.Common.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class DisableAutoXR : MonoBehaviour
    {
        public bool @default;
        private bool _current;

        void Start()
        {
            DisableXR(@default);
        }

        public void DisableXR(bool disable)
        {
            _current = disable;
            XRDevice.DisableAutoXRCameraTracking(GetComponent<Camera>(), disable);
        }

        void OnEnable()
        {
            DisableXR(_current);
        }
    }
}