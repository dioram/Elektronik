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

        public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        public static IObservable<int> OnValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.CreateWithState<int, TMP_Dropdown>(dropdown, (i, observer) =>
            {
                observer.OnNext(i.value);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }


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
        public static IObservable<string> OnSubmitAsObservable(this TMP_InputField inputField)
        {
            var subject = new Subject<string>();
            inputField.onSubmit.AddListener(v => subject.OnNext(v));
            return subject;
        }

        public static IObservable<Unit> RepeatEveryUpdateUntilEventOrDestroyed<T>(
            this IObservable<Unit> observable, IObservable<T> @event, MonoBehaviour obj)
        {
            return observable.SelectMany(v => obj.gameObject.UpdateAsObservable())
                    .TakeUntil(@event)
                    .RepeatUntilDestroy(obj);
        }

        public static IObservable<T> RepeatEveryUpdateUntilEventOrDestroyed<T, T1>(
            this IObservable<T> observable, IObservable<T1> @event, MonoBehaviour obj)
        {
            return observable.SelectMany(v => obj.gameObject.UpdateAsObservable().Select(_ => v))
                    .TakeUntil(@event)
                    .RepeatUntilDestroy(obj);
        }
    }
}