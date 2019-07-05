using System;
using UnityEngine;

namespace Elektronik.Common.Cameras
{
    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class OrbitalCameraForPointInSpace : MonoBehaviour
    {
        private Vector3 m_targetPosition;
        private Quaternion m_targetRotation;
        public float fly2TargetDistanceDelta = .25f;

        public float distance = 10.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -80f;
        public float yMaxLimit = 80f;

        public float distanceMin = .5f;
        public float distanceMax = 15f;

        float x = 0.0f;
        float y = 0.0f;

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
                m_targetPosition = to;
                transform.position = from.position;
                transform.rotation = from.rotation;
                gameObject.SetActive(true);
            }
        }

        private void OnPosition()
        {
            if (CurrentState == State.FlyingToPosition)
            {
                if (Vector3.Distance(transform.position, m_targetPosition) < 1.0f)
                {
                    CurrentState = State.Active;
                }
            }
        }

        public void SwitchOff()
        {
            gameObject.SetActive(false);
            CurrentState = State.Inactive;
            if (OnSwitchOff != null)
                OnSwitchOff();
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
                float currentDistance = Vector3.Distance(transform.position, m_targetPosition);
                m_targetRotation = Quaternion.LookRotation((m_targetPosition - transform.position).normalized);
                transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, fly2TargetDistanceDelta * currentDistance * .05f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, fly2TargetDistanceDelta + (1 / currentDistance));
                if (currentDistance <= distance)
                {
                    CurrentState = State.Active;
                    x = transform.rotation.eulerAngles.y;
                    y = transform.rotation.eulerAngles.x;
                }
            }
            if (CurrentState == State.Active)
            {
                if (Input.GetKeyDown(KeyCode.End))
                {
                    Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ?
                        CursorLockMode.None :
                        CursorLockMode.Locked;
                }

                if (Input.GetMouseButton(1))
                {
                    x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                    y = ClampAngle(y, yMinLimit, yMaxLimit);
                }

                Quaternion rotation = Quaternion.Euler(y, x, 0);
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                RaycastHit hit;
                if (Physics.Linecast(m_targetPosition, transform.position, out hit))
                {
                    distance -= hit.distance;
                }
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + m_targetPosition;
                transform.rotation = rotation;
                transform.position = position;
            }
        }
    }
}
