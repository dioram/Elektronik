using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public class ObjectLogger : MonoBehaviour
    {
        public GameObject arrow;
        public SpecialInformationBanner specialInformationBanner;
        public OrbitalCameraForPointInSpace specialCamera;

        public void ShowObjectInformation(string information, Vector3 objectPosition)
        {
            specialInformationBanner.gameObject.SetActive(true);
            specialInformationBanner.SetText(information);
            arrow.SetActive(true);
            arrow.transform.position = objectPosition + new Vector3(0, 1, 0);
            Camera currentCam = Camera.current;
            currentCam.gameObject.SetActive(false);
            specialCamera.FlyToPosition(currentCam.transform.position, objectPosition);
            specialCamera.OnSwitchOff += () => currentCam.gameObject.SetActive(true);
            specialCamera.OnSwitchOff += () => specialInformationBanner.Clear();
            specialCamera.OnSwitchOff += () => specialInformationBanner.gameObject.SetActive(false);
            specialCamera.OnSwitchOff += () => arrow.SetActive(false);
        }
    }
}
