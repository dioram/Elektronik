using Elektronik.Common.Maps;
using UnityEngine;

namespace Elektronik.Online.VRHacker
{
    class VRHacker : MonoBehaviour
    {
        public SlamMap map;
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
            if (map.TrackedObjs.Count > 0)
            {
                var obj = map.TrackedObjs[0];
                headset.rotation = obj.rotation;
                headset.localPosition = obj.position + new Vector3(0, 1, 0);
            }
        }
    }
}
