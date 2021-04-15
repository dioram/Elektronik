using UnityEngine;
using UnityEngine.XR;

namespace Elektronik.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class DisableAutoXR : MonoBehaviour
    {
        public bool Default;
        private bool _current;

        void Start()
        {
            DisableXR(Default);
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