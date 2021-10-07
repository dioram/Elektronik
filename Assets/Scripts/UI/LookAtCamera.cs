using UnityEngine;

namespace Elektronik.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        public bool Reverse;
        private Transform _cameraTransform;

        public float Radius = 0;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (Reverse)
            {
                transform.LookAt(transform.position * 2 - _cameraTransform.position, _cameraTransform.up);
            }
            else
            {
                transform.LookAt(_cameraTransform, _cameraTransform.up);
            }
            
            transform.position = transform.parent.position + _cameraTransform.up * Radius;
        }
    }
}