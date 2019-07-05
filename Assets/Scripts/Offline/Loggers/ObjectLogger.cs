using Elektronik.Offline.UI;
using Elektronik.Common.Cameras;
using UnityEngine;

namespace Elektronik.Offline.Loggers
{
    public class ObjectLogger : MonoBehaviour
    {
        public GameObject arrow;
        public SpecialInformationBanner specialInformationBanner;
        public OrbitalCameraForPointInSpace specialCamera;

        public void ShowObjectInformation(string information, Vector3 objectPosition)
        {
            if (specialCamera.CurrentState != OrbitalCameraForPointInSpace.State.Inactive)
            {
                specialCamera.SwitchOff();
            }
            specialInformationBanner.gameObject.SetActive(true);
            specialInformationBanner.SetText(information);
            arrow.SetActive(true);
            arrow.transform.position = objectPosition + new Vector3(0, .45f, 0);
            Camera currentCam = Camera.allCameras[0];
            currentCam.gameObject.SetActive(false);
            specialCamera.FlyToPosition(currentCam.transform, objectPosition);
            specialCamera.OnSwitchOff += () => currentCam.gameObject.SetActive(true);
            specialCamera.OnSwitchOff += () => specialInformationBanner.Clear();
            specialCamera.OnSwitchOff += () => specialInformationBanner.gameObject.SetActive(false);
            specialCamera.OnSwitchOff += () => arrow.SetActive(false);
        }
    }
}
