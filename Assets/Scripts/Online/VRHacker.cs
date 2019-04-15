using UnityEngine;

namespace Elektronik.Online
{
    class VRHacker : MonoBehaviour
    {
        public Transform helmet;
        public Transform headset;

        private void OnEnable()
        {
            Debug.Log("Hacker enabled");
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(headset.gameObject.GetComponent<Camera>(), true);
        }

        private void OnDisable()
        {
            Debug.Log("Hacker disabled");
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(headset.gameObject.GetComponent<Camera>(), false);
        }

        private void Update()
        {
            headset.rotation = helmet.rotation;
            headset.localPosition = helmet.position + new Vector3(0, 1, 0);
        }
    }
}
