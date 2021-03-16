using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elektronik.Common.Cameras
{
    public class CameraChanger : MonoBehaviour
    {
        [Serializable]
        public sealed class CameraChangerEvent : UnityEvent<int> { }

        public GameObject[] cameras;

        public CameraChangerEvent onValueChanged = new CameraChangerEvent();

        public void SetCamera(int num)
        {
            Array.ForEach(cameras, c => c.SetActive(false));
            cameras[num].SetActive(true);
            onValueChanged.Invoke(num);
        }
    }
}
