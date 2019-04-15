using UnityEngine;

namespace Elektronik.Online
{
    class VRHacker_New : MonoBehaviour
    {
        public Transform helmet;
        public Transform headset;

        private bool m_positionalTracking;
        
        
        private void OnEnable()
        {
            m_positionalTracking = UnityEngine.XR.InputTracking.disablePositionalTracking;
            UnityEngine.XR.InputTracking.disablePositionalTracking = true;
            Debug.Log("Hacker enabled");
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(headset.gameObject.GetComponent<Camera>(), true);
        }

        private void OnDisable()
        {
            UnityEngine.XR.InputTracking.disablePositionalTracking = m_positionalTracking;;
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
