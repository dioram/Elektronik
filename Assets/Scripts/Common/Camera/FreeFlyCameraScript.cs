using UnityEngine;

namespace Elektronik.Common.Cameras
{
    public class FreeFlyCameraScript : MonoBehaviour
    {
        public float cameraSensitivity = 90;
        public float climbSpeed = 4;
        public float normalMoveSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3;

        private Vector3 m_rotation;

        public void Update()
        {
            if (Input.GetMouseButton(1))
            {
                m_rotation.x += Input.GetAxis("Mouse X") * cameraSensitivity * Time.fixedDeltaTime;
                m_rotation.y += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.fixedDeltaTime;
                m_rotation.y = Mathf.Clamp(m_rotation.y, -80, 80);
            }

            if (Input.GetKey(KeyCode.Keypad4))
                m_rotation.x -= .1f * cameraSensitivity * Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.Keypad6))
                m_rotation.x += .1f * cameraSensitivity * Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.Keypad8))
            {
                m_rotation.y += .1f * cameraSensitivity * Time.fixedDeltaTime;
                m_rotation.y = Mathf.Clamp(m_rotation.y, -80, 80);
            }
            if (Input.GetKey(KeyCode.Keypad5))
            {
                m_rotation.y -= .1f * cameraSensitivity * Time.fixedDeltaTime;
                m_rotation.y = Mathf.Clamp(m_rotation.y, -80, 80);
            }
            if (Input.GetKey(KeyCode.Keypad7))
                m_rotation.z += .1f * cameraSensitivity * Time.fixedDeltaTime;
            if (Input.GetKey(KeyCode.Keypad9))
                m_rotation.z -= .1f * cameraSensitivity * Time.fixedDeltaTime;

            if (Input.GetKey(KeyCode.Keypad0))
                m_rotation = Vector3.zero;

            transform.localRotation = Quaternion.AngleAxis(m_rotation.x, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(m_rotation.z, Vector3.forward);
            transform.localRotation *= Quaternion.AngleAxis(m_rotation.y, Vector3.left);

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor * Input.GetAxis("Vertical") * Time.deltaTime);
                transform.position += transform.right * (normalMoveSpeed * fastMoveFactor * Input.GetAxis("Horizontal") * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor * Input.GetAxis("Vertical") * Time.deltaTime);
                transform.position += transform.right * (normalMoveSpeed * slowMoveFactor * Input.GetAxis("Horizontal") * Time.deltaTime);
            }
            else
            {
                transform.position += transform.forward * (normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime);
                transform.position += transform.right * (normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * (climbSpeed * Time.deltaTime); }
            if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * (climbSpeed * Time.deltaTime); }

            if (Input.GetKeyDown(KeyCode.End))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ?
                    CursorLockMode.None :
                    CursorLockMode.Locked;
            }
        }
    }
}


