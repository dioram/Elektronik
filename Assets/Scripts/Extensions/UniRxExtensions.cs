using System;
using TMPro;

// ReSharper disable once CheckNamespace
namespace UniRx
{
    public static class UniRxExtensions
    {
        public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
    }
}