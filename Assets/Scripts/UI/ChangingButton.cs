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

        public Action<int> OnStateChanged;

        public int State
        {
            get => _state;
            set
            {
                if (_state >= 0 && _state < Events.Count) Events[_state]?.Invoke();
                if (_state == value % MaxState) return;
                _state = value % MaxState;
                SetValue();
                OnStateChanged?.Invoke(State);
            }
        }

        public void InitState(int state)
        {
            _state = state == 0 ? MaxState - 1 : state - 1;
            State = state;
            _wasInited = true;
        }

        #region Unity events

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _button.OnClickAsObservable().Do(_ => State++).Subscribe();
        }

        protected virtual void Start()
        {
            if (_wasInited) return;
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
        private bool _wasInited;

        #endregion
    }
}