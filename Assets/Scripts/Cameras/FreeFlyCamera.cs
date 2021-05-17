//===========================================================================//
//                       FreeFlyCamera (Version 1.2)                         //
//                        (c) 2019 Sergey Stafeyev                           //
//===========================================================================//

using UnityEngine;

namespace Elektronik.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class FreeFlyCamera : MonoBehaviour
    {
        #region UI

        [Space] [SerializeField] [Tooltip("The script is currently active")]
        private bool Active = true;

        [Space] [SerializeField] [Tooltip("Camera rotation by mouse movement is active")]
        private bool EnableRotation = true;

        [SerializeField] [Tooltip("Sensitivity of mouse rotation")]
        private float MouseSense = 1.8f;

        [SerializeField] [Tooltip("Sensitivity of keyboard rotation")] [Range(0, 10)]
        private float KeyboardRotationSense = 1.8f;

        [Space] [SerializeField] [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
        private bool EnableTranslation = true;

        [SerializeField] [Tooltip("Velocity of camera zooming in/out")]
        private float TranslationSpeed = 55f;

        [Space] [SerializeField] [Tooltip("Camera movement by 'W','A','S','D','Q','E' keys is active")]
        private bool EnableMovement = true;

        [SerializeField] [Tooltip("Camera movement speed")]
        private float MovementSpeed = 10f;

        [SerializeField] [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
        private float BoostedSpeed = 50f;

        [SerializeField] [Tooltip("Boost speed")]
        private KeyCode BoostSpeed = KeyCode.LeftShift;

        [SerializeField] [Tooltip("Move up")] private KeyCode MoveUp = KeyCode.Q;

        [SerializeField] [Tooltip("Move down")]
        private KeyCode MoveDown = KeyCode.E;

        [Space] [SerializeField] [Tooltip("Acceleration at camera movement is active")]
        private bool EnableSpeedAcceleration = true;

        [SerializeField] [Tooltip("Rate which is applied during camera movement")]
        private float SpeedAccelerationFactor = 1.5f;

        [Space] [SerializeField] [Tooltip("This keypress will move the camera to initialization position")]
        private KeyCode InitPositionButton = KeyCode.R;

        #endregion UI

        private float _currentIncrease = 1;
        private float _currentIncreaseMem = 0;

        private Vector3 _initPosition;
        private Vector3 _initRotation;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (BoostedSpeed < MovementSpeed) BoostedSpeed = MovementSpeed;
        }
#endif


        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.eulerAngles;
        }

        private void CalculateCurrentIncrease(bool moving)
        {
            _currentIncrease = Time.deltaTime;

            if (!EnableSpeedAcceleration || EnableSpeedAcceleration && !moving)
            {
                _currentIncreaseMem = 0;
                return;
            }

            _currentIncreaseMem += Time.deltaTime * (SpeedAccelerationFactor - 1);
            _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
        }

        private void Update()
        {
            if (!Active) return;

            // Translation
            if (EnableTranslation)
            {
                transform.Translate(Vector3.forward * (Input.mouseScrollDelta.y * Time.deltaTime * TranslationSpeed));
            }

            // Movement
            if (EnableMovement)
            {
                Vector3 deltaPosition = Vector3.zero;
                float currentSpeed = MovementSpeed;

                if (Input.GetKey(BoostSpeed)) currentSpeed = BoostedSpeed;

                if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(0) && Input.GetMouseButton(1)) 
                    deltaPosition += transform.forward;

                if (Input.GetKey(KeyCode.S)) deltaPosition -= transform.forward;

                if (Input.GetKey(KeyCode.A)) deltaPosition -= transform.right;

                if (Input.GetKey(KeyCode.D)) deltaPosition += transform.right;

                if (Input.GetKey(MoveUp)) deltaPosition += transform.up;

                if (Input.GetKey(MoveDown)) deltaPosition -= transform.up;

                // Calc acceleration
                CalculateCurrentIncrease(deltaPosition != Vector3.zero);

                transform.position += deltaPosition * (currentSpeed * _currentIncrease);
            }

            // Rotation
            if (EnableRotation)
            {
                if (Input.GetMouseButton(1))
                {
                    // Pitch
                    transform.rotation *= Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * MouseSense,
                                                               Vector3.right);

                    // Yaw
                    transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                          transform.eulerAngles.y 
                                                          + Input.GetAxis("Mouse X") * MouseSense,
                                                          transform.eulerAngles.z);
                }
                else
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                        transform.rotation *= Quaternion.AngleAxis(KeyboardRotationSense, Vector3.right);

                    if (Input.GetKey(KeyCode.DownArrow))
                        transform.rotation *= Quaternion.AngleAxis(-KeyboardRotationSense, Vector3.right);

                    if (Input.GetKey(KeyCode.LeftArrow))
                        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                              transform.eulerAngles.y - KeyboardRotationSense,
                                                              transform.eulerAngles.z);

                    if (Input.GetKey(KeyCode.RightArrow))
                        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                              transform.eulerAngles.y + KeyboardRotationSense,
                                                              transform.eulerAngles.z);
                }
            }

            // Return to init position
            if (Input.GetKeyDown(InitPositionButton))
            {
                transform.position = _initPosition;
                transform.eulerAngles = _initRotation;
            }
        }
    }
}