using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class ChangingButton : MonoBehaviour
    {
        [Serializable]
        public sealed class ButtonPressedEvent : UnityEvent { }

        public List<ButtonPressedEvent> Events = new List<ButtonPressedEvent>();
        
        public IObservable<int> OnClickAsObservable() => _button.OnClickAsObservable().Select(_ => State);

        public int State
        {
            get => _state;
            set
            {
                _state = value % MaxState;
                if (State < Events.Count) Events[State]?.Invoke();
                SetValue();
            }
        }

        #region Unity events

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.OnClickAsObservable().Subscribe(_ => State++);
        }

        protected virtual void Start()
        {
            _state = 0;
            SetValue();
        }

        #endregion

        #region Protected

        protected int MaxState = 1;

        protected abstract void SetValue();

        #endregion

        #region Private

        private Button _button;
        private int _state;

        #endregion
    }
}