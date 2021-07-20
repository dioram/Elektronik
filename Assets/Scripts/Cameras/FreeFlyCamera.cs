using UnityEngine;

namespace Elektronik.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class FreeFlyCamera : MonoBehaviour
    {
        #region UI

        [Space] [SerializeField] [Tooltip("Is camera rotation by mouse movement active?")]
        private bool RotationEnabled = true;

        [SerializeField] [Tooltip("Sensitivity of mouse rotation")] [Range(0, 5)]
        private float MouseSense = 1.8f;

        [SerializeField] [Tooltip("Sensitivity of keyboard rotation")]
        private float KeyboardRotationSense = 15f;

        [Space] [SerializeField] [Tooltip("Is camera zooming in/out by 'Mouse Scroll Wheel' active?")]
        private bool UseMouseWheel = true;

        [SerializeField] [Tooltip("Velocity of camera zooming in/out")]
        private float MouseWheelSpeed = 55f;

        [Space] [SerializeField] [Tooltip("Is camera movement by 'W','A','S','D','Q','E' keys active?")]
        private bool MovementEnabled = true;

        [SerializeField] [Tooltip("Speed of the camera movement")]
        private float DefaultMovementMultiplier = 5;

        [SerializeField] [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
        private float BoostedMultiplier = 10;

        #endregion UI

        private Vector3 _initPosition;
        private Vector3 _initRotation;

        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.eulerAngles;
        }

        private void Move()
        {
            float currentSpeed = DefaultMovementMultiplier;
            if (Input.GetAxis("Boost") > 0) currentSpeed = BoostedMultiplier;

            var forward = Input.GetAxis("LMB") > 0 && Input.GetAxis("RMB") > 0 ? 1 : Input.GetAxis("Forward");
            if (forward == 0 && UseMouseWheel)
            {
                forward = Input.mouseScrollDelta.y * MouseWheelSpeed;
            }

            var right = Input.GetAxis("Right");
            var up = Input.GetAxis("Up");

            var deltaPosition = (transform.forward * forward + transform.right * right + transform.up * up)
                    * Time.deltaTime;

            transform.position += deltaPosition * currentSpeed;
        }

        private void Rotate()
        {
            if (Input.GetAxis("RMB") > 0)
            {
                // Pitch
                transform.rotation *=
                        Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * MouseSense * Time.deltaTime, Vector3.right);

                // Yaw
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                      transform.eulerAngles.y
                                                      + Input.GetAxis("Mouse X") * MouseSense * Time.deltaTime,
                                                      transform.eulerAngles.z);
                return;
            }

            if (Input.GetAxis("Pitch") != 0)
            {
                transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Pitch")
                                                           * KeyboardRotationSense * Time.deltaTime,
                                                           Vector3.right);
            }

            if (Input.GetAxis("Yaw") != 0)
            {
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                      transform.eulerAngles.y
                                                      + Input.GetAxis("Yaw") * KeyboardRotationSense * Time.deltaTime,
                                                      transform.eulerAngles.z);
            }
        }

        private void Update()
        {
            // Movement
            if (MovementEnabled) Move();

            // Rotation
            if (RotationEnabled) Rotate();

            // Return to init position
            if (Input.GetAxis("Reset") > 0)
            {
                transform.position = _initPosition;
                transform.eulerAngles = _initRotation;
            }
        }
    }
}