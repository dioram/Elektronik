﻿using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Elektronik.UI.Buttons
{
    // TODO: rewrite and simplify
    
    [RequireComponent(typeof(Button))]
    public abstract class ChangingButton : MonoBehaviour
    {
        [Serializable]
        public sealed class ButtonPressedEvent : UnityEvent
        {
        }
        
        #region Editor fields

        public List<ButtonPressedEvent> Events = new List<ButtonPressedEvent>();

        #endregion

        public event Action<int> OnStateChanged;
        public IObservable<int> OnStateChangedAsObservable;

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

        public void SilentSetState(int state)
        {
            if (_state >= 0 && _state < Events.Count) Events[_state]?.Invoke();
            if (_state == state % MaxState) return;
            _state = state % MaxState;
            SetValue();
        }

        public void Toggle()
        {
            State++;
        }

        #region Unity events

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
            _button.OnClickAsObservable().Subscribe(_ => Toggle());

            OnStateChangedAsObservable = Observable.FromEvent<int>(h => OnStateChanged += h, h => OnStateChanged -= h);
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