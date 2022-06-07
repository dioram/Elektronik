using System;
using System.Security;
using TMPro;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

// ReSharper disable once CheckNamespace
namespace UniRx
{
    public static class UniRxExtensions
    {
        #region Threading

        // TODO: Use UniTask instead of this

        /// <summary> Launches given action in main thread. </summary>
        /// <param name="action"> Action that will be run in main thread. </param>
        /// <returns> Observable on action's result. </returns>
        public static IObservable<Unit> StartOnMainThread(Action action)
        {
            try
            {
                return Observable.Start(action, Scheduler.MainThread);
            }
            catch (SecurityException)
            {
                return Observable.Empty<Unit>();
            }
        }

        /// <summary> Launches given function in main thread. </summary>
        /// <param name="func"> Function that will be run in main thread. </param>
        /// <returns> Observable on function's result. </returns>
        public static IObservable<T> StartOnMainThread<T>(Func<T> func)
        {
            try
            {
                return Observable.Start(func, Scheduler.MainThread);
            }
            catch (SecurityException)
            {
                return Observable.Empty<T>();
            }
        }

        /// <summary>
        /// This extension returns <c>Observable.Empty/<T/></c> in case of <see cref="SecurityException"/>.
        /// </summary>
        public static IObservable<T> ObserveOnMainThreadSafe<T>(this IObservable<T> obs)
        {
            try
            {
                return obs.ObserveOnMainThread();
            }
            catch (SecurityException)
            {
                return Observable.Empty<T>();
            }
        }

        #endregion

        #region InputAction

        public static IObservable<InputContext> PerformedAsObservable(this InputAction inputAction) =>
                Observable.FromEvent<InputContext>(h => inputAction.performed += h,
                                                   h => inputAction.performed -= h);

        public static IObservable<InputContext> StartedAsObservable(this InputAction inputAction) =>
                Observable.FromEvent<InputContext>(h => inputAction.started += h,
                                                   h => inputAction.started -= h);

        public static IObservable<InputContext> CanceledAsObservable(this InputAction inputAction) =>
                Observable.FromEvent<InputContext>(h => inputAction.canceled += h,
                                                   h => inputAction.canceled -= h);

        public static IObservable<InputContext> CanceledAsObservableWithDefaultState(this InputAction inputAction)
        {
            var events = Observable.FromEvent<InputContext>(
                h => inputAction.canceled += h,
                h => inputAction.canceled -= h);

            return Observable.CreateWithState(new InputContext(),
                                              (InputContext context, IObserver<InputContext> observer) =>
                                              {
                                                  observer.OnNext(context);
                                                  return events.Subscribe(observer);
                                              });
        }

        #endregion

        #region TMP_InputField

        public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        public static IObservable<string> OnSubmitAsObservable(this TMP_InputField inputField)
        {
            var subject = new Subject<string>();
            inputField.onSubmit.AddListener(v => subject.OnNext(v));
            return subject;
        }

        #endregion

        #region Observable

        /// <summary> Alias for popular rx pattern. </summary>
        public static IObservable<Unit> RepeatEveryUpdateUntilEventOrDestroyed<T>(
            this IObservable<Unit> observable, IObservable<T> @event, MonoBehaviour obj)
        {
            return observable.SelectMany(v => obj.gameObject.UpdateAsObservable())
                    .TakeUntil(@event)
                    .RepeatUntilDestroy(obj);
        }

        /// <summary> Alias for popular rx pattern. </summary>
        public static IObservable<T> RepeatEveryUpdateUntilEventOrDestroyed<T, T1>(
            this IObservable<T> observable, IObservable<T1> @event, MonoBehaviour obj)
        {
            return observable.SelectMany(v => obj.gameObject.UpdateAsObservable().Select(_ => v))
                    .TakeUntil(@event)
                    .RepeatUntilDestroy(obj);
        }

        #endregion
    }
}