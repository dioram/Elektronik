using System;
using UnityEngine;

namespace Elektronik.Cameras
{
    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class OrbitalCameraForPointInSpace : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        public float Fly2TargetDistanceDelta = .25f;

        public float Distance = 10.0f;
        public float XSpeed = 120.0f;
        public float YSpeed = 120.0f;

        public float YMinLimit = -80f;
        public float YMaxLimit = 80f;

        public float DistanceMin = .5f;
        public float DistanceMax = 15f;

        float _x = 0.0f;
        float _y = 0.0f;

        public event Action OnSwitchOff;

        public enum State
        {
            Inactive,
            Active,
            FlyingToPosition,
        }

        public State CurrentState { get; private set; }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        public void FlyToPosition(Transform @from, Vector3 to)
        {
            if (CurrentState == State.Inactive)
            {
                CurrentState = State.FlyingToPosition;
                _targetPosition = to;
                transform.position = from.position;
                transform.rotation = from.rotation;
                gameObject.SetActive(true);
            }
        }

        private void OnPosition()
        {
            if (CurrentState == State.FlyingToPosition)
            {
                if (Vector3.Distance(transform.position, _targetPosition) < 1.0f)
                {
                    CurrentState = State.Active;
                }
            }
        }

        public void SwitchOff()
        {
            gameObject.SetActive(false);
            CurrentState = State.Inactive;
            OnSwitchOff?.Invoke();
            OnSwitchOff = delegate { };
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D))
            {
                SwitchOff();
            }

            if (CurrentState == State.FlyingToPosition)
            {
                float currentDistance = Vector3.Distance(transform.position, _targetPosition);
                _targetRotation = Quaternion.LookRotation((_targetPosition - transform.position).normalized);
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition,
                                                         Fly2TargetDistanceDelta * currentDistance * .05f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation,
                                                              Fly2TargetDistanceDelta + (1 / currentDistance));
                if (currentDistance <= Distance)
                {
                    CurrentState = State.Active;
                    _x = transform.rotation.eulerAngles.y;
                    _y = transform.rotation.eulerAngles.x;
                }
            }

            if (CurrentState == State.Active)
            {
                if (Input.GetKeyDown(KeyCode.End))
                {
                    Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                            ? CursorLockMode.None
                            : CursorLockMode.Locked;
                }

                if (Input.GetMouseButton(1))
                {
                    _x += Input.GetAxis("Mouse X") * XSpeed * Distance * 0.02f;
                    _y -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;
                    _y = ClampAngle(_y, YMinLimit, YMaxLimit);
                }

                Quaternion rotation = Quaternion.Euler(_y, _x, 0);
                Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * 5, DistanceMin, DistanceMax);
                RaycastHit hit;
                if (Physics.Linecast(_targetPosition, transform.position, out hit))
                {
                    Distance -= hit.distance;
                }

                Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
                Vector3 position = rotation * negDistance + _targetPosition;
                transform.rotation = rotation;
                transform.position = position;
            }
        }
    }
}