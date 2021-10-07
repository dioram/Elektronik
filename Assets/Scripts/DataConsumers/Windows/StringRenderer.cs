using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.Windows
{
    public class StringRenderer : MonoBehaviour, IDataRenderer<string>
    {
        public enum Modes
        {
            Append,
            ShowLast
        }

        public Modes Mode = Modes.ShowLast;

        #region Unity events

        private void OnEnable()
        {
            IsShowing = true;
        }

        private void OnDisable()
        {
            IsShowing = false;
        }

        #endregion

        #region IDataRenderer

        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                if (_isShowing == value) return;
                _isShowing = value;
                UniRxExtensions.StartOnMainThread(() => gameObject.SetActive(_isShowing)).Subscribe();
            }
        }

        public float Scale { get; set; }

        public void Render(string data)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                switch (Mode)
                {
                case Modes.Append:
                    Label.text += "\n---------------------------\n";
                    Label.text += data;
                    break;
                case Modes.ShowLast:
                    Label.text = data;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }).Subscribe();
        }

        public void Clear()
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                if (Label != null) Label.text = "";
            }).Subscribe();
        }

        #endregion

        #region Private

        [SerializeField] private TMP_Text Label;
        private bool _isShowing;

        #endregion
    }
}