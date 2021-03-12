using System;
using UnityEngine;

namespace Elektronik.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        public bool Reverse;

        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (Reverse)
            {
                transform.LookAt(transform.position * 2 - _cameraTransform.position);
            }
            else
            {
                transform.LookAt(_cameraTransform);
            }
        }
    }
}