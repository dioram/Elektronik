using Elektronik.Input;
using UniRx;
using UnityEngine;

namespace Elektronik.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class FreeFlyCamera : MonoBehaviour
    {
        private Vector3 _initPosition;
        private Quaternion _initRotation;

        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.rotation;

            var controls = new CameraControls().Controls;
            controls.Enable();
            
            var boost = new[]
                    {
                        controls.Boost.StartedAsObservable(),
                        controls.Boost.CanceledAsObservableWithDefaultState(),
                    }
                    .Merge()
                    .Select(v => v.ReadValue<float>())
                    .Select(v => v > 0 ? v : 1);

            var move = new[]
            {
                controls.MoveForward.StartedAsObservable()
                        .Select(v => Vector3.forward * v.ReadValue<float>())
                        .RepeatEveryUpdateUntilEventOrDestroyed(controls.MoveForward.CanceledAsObservable(), this),
                controls.MoveScroll.PerformedAsObservable()
                        .Select(v => Vector3.forward * v.ReadValue<float>()),
                controls.MoveSides.StartedAsObservable()
                        .Select(v => (Vector3)v.ReadValue<Vector2>())
                        .RepeatEveryUpdateUntilEventOrDestroyed(controls.MoveSides.CanceledAsObservable(), this),
                controls.MoveMouseDrag.PerformedAsObservable()
                        .Select(v => (Vector3)v.ReadValue<Vector2>()),
            };
            
            move.Merge()
                    .CombineLatest(boost, (v, f) => v * f)
                    .Subscribe(Move)
                    .AddTo(this);


            var rotate = new[]
            {
                controls.RotateRoll.StartedAsObservable()
                        .Select(v => Vector3.forward * v.ReadValue<float>())
                        .RepeatEveryUpdateUntilEventOrDestroyed(controls.RotateRoll.CanceledAsObservable(), this),
                controls.Rotate.StartedAsObservable()
                        .Select(v => (Vector3)v.ReadValue<Vector2>())
                        .RepeatEveryUpdateUntilEventOrDestroyed(controls.Rotate.CanceledAsObservable(), this),
                controls.RotateMouseDrag.PerformedAsObservable()
                        .Select(v => (Vector3)v.ReadValue<Vector2>()),
            };

            rotate.Merge()
                    .Subscribe(Rotate)
                    .AddTo(this);

            controls.Reset.PerformedAsObservable()
                    .Subscribe(_ => Reset())
                    .AddTo(this);
        }

        private void Move(Vector3 delta)
        {
            var deltaPosition = (transform.forward * delta.z + transform.right * delta.x + transform.up * delta.y)
                    * Time.fixedDeltaTime;

            transform.position += deltaPosition;
        }

        private void Reset()
        {
            transform.position = _initPosition;
            transform.rotation = _initRotation;
        }

        private void Rotate(Vector3 rotation)
        {
            transform.rotation *= Quaternion.AngleAxis(rotation.y * Time.fixedDeltaTime, Vector3.right);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                  transform.eulerAngles.y + rotation.x * Time.fixedDeltaTime,
                                                  transform.eulerAngles.z);
            transform.Rotate(Vector3.forward, rotation.z, Space.Self);
        }
    }
}