using Elektronik.Common.Maps;
using UnityEngine;

namespace Elektronik.Online.VRHacker
{
    class VRHacker : MonoBehaviour
    {
        public TracksOwner helmetsOwner;
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
            if (helmetsOwner.Helmets.Count > 0)
            {
                Helmet helmet = helmetsOwner.Helmets[0];
                headset.rotation = helmet.transform.rotation;
                headset.localPosition = helmet.transform.position + new Vector3(0, 1, 0);
            }
        }
    }
}
