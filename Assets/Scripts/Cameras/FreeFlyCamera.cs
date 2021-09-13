using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class FreeFlyCamera : MonoBehaviour
    {
        private Vector3 _movement = Vector3.zero;
        private Vector2 _rotation = Vector2.zero;
        private float _speed = 1;
        private Vector3 _initPosition;
        private Quaternion _initRotation;

        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.rotation;
        }

        #region Input events

        public void OnMoveForward(InputValue input)
        {
            _movement.z = input.Get<float>();
        }

        public void OnMoveSides(InputValue input)
        {
            var vec = input.Get<Vector2>();
            _movement.x = vec.x;
            _movement.y = vec.y;
        }

        public void OnRotate(InputValue input)
        {
            var vec = input.Get<Vector2>();
            _rotation.x = vec.x;
            _rotation.y = vec.y;
        }

        public void OnBoost(InputValue input)
        {
            var value = input.Get<float>();
            _speed = value > 0 ? value : 1;
        }

        public void OnReset()
        {
            transform.position = _initPosition;
            transform.rotation = _initRotation;
        }

        #endregion

        private void FixedUpdate()
        {
            var deltaPosition = (transform.forward * _movement.z 
                + transform.right * _movement.x 
                + transform.up * _movement.y) * (Time.fixedDeltaTime * _speed);

            transform.position += deltaPosition;
            
            transform.rotation *= Quaternion.AngleAxis(_rotation.y * Time.fixedDeltaTime, Vector3.right);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                  transform.eulerAngles.y + _rotation.x * Time.fixedDeltaTime,
                                                  transform.eulerAngles.z);
        }
    }
}