using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.Events;

namespace Elektronik.Common
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
