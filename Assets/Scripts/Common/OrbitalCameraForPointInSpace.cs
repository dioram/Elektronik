using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class OrbitalCameraForPointInSpace : MonoBehaviour
    {
        private Vector3 target;
        public float fly2TargetDistanceDelta = .25f;

        public float distance = 5.0f;
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

        State m_state;

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        public void FlyToPosition(Vector3 @from, Vector3 to, Quaternion orientation)
        {
            if (m_state == State.Inactive)
            {
                m_state = State.FlyingToPosition;
                target = to;
                transform.position = @from;
                transform.rotation = orientation;
            }
        }

        private void OnPosition()
        {
            if (m_state == State.FlyingToPosition)
            {
                if (Vector3.Distance(transform.position, target) < 1.0f)
                {
                    m_state = State.Active;
                }
            }
        }

        public void SwitchOff()
        {
            gameObject.SetActive(false);
            m_state = State.Inactive;
            if (OnSwitchOff != null)
                OnSwitchOff();
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
            if (isActiveAndEnabled && m_state == State.FlyingToPosition)
            {
                Vector3.MoveTowards(transform.position, target, fly2TargetDistanceDelta);
            }
            if (isActiveAndEnabled && m_state == State.Active)
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
                distance -= Vector3.Distance(transform.position, target);
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target;
                transform.rotation = rotation;
                transform.position = position;
            }
        }
    }
}
