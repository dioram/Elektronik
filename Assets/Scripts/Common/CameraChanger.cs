using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

namespace Elektronik.Common
{
    [RequireComponent(typeof(Dropdown))]
    public class CameraChanger : MonoBehaviour
    {
        public GameObject[] cameras;
        private Dropdown m_dropdown;

        void Start()
        {
            m_dropdown = GetComponent<Dropdown>();
            
            m_dropdown.OnValueChangedAsObservable()
                .Do(_ => Array.ForEach(cameras, c => c.SetActive(false)))
                .Do(id => cameras[id].SetActive(true))
                .Subscribe();
        }
    }
}
