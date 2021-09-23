using System;
using UniRx;
using UnityEngine.InputSystem;

namespace Elektronik.Input
{
    public static class InputActionExtensions
    {
        public static IObservable<InputAction.CallbackContext> PerformedAsObservable(this InputAction inputAction) =>
                Observable.FromEvent<InputAction.CallbackContext>(h => inputAction.performed += h,
                                                                  h => inputAction.performed -= h);
    }
}