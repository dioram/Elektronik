using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common
{
    public class FreeFlyCameraScript : MonoBehaviour
    {
        public float cameraSensitivity = 90;
        public float climbSpeed = 4;
        public float normalMoveSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3;

        private float _rotationX;
        private float _rotationY;

        public void ResetRotation()
        {
            Vector3 rotations = transform.localRotation.eulerAngles;
            _rotationY = rotations.x > 180 ? 360 - rotations.x : -rotations.x;
            _rotationX = rotations.y > 180 ? rotations.y - 360 : rotations.y;
        }

        public void Update()
        {
            if (Input.GetMouseButton(1))
            {
                _rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.fixedDeltaTime;
                _rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.fixedDeltaTime;
                _rotationY = Mathf.Clamp(_rotationY, -80, 80);
            }

            transform.localRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.fixedDeltaTime;
                transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.fixedDeltaTime;
                transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
            }
            else
            {
                transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.fixedDeltaTime; }
            if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.fixedDeltaTime; }

            if (Input.GetKeyDown(KeyCode.End))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ?
                    CursorLockMode.None :
                    CursorLockMode.Locked;
            }
        }
    }
}


