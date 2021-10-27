using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.Windows
{
    /// <summary> This class renders string to window. </summary>
    public class StringRenderer : MonoBehaviour, IDataRenderer<string>
    {
        #region Editor fields

        /// <summary> Label where text will be rendered. </summary>
        [SerializeField] [Tooltip("Label where text will be rendered.")]
        private TMP_Text Label;

        #endregion

        /// <summary> How to handle messages </summary>
        public enum Modes
        {
            /// <summary> Next messages appends previous. </summary>
            Append,

            /// <summary> Show only last one message. </summary>
            ShowLast
        }

        /// <summary> Mode of handling incoming messages. </summary>
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Clear()
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                if (Label != null) Label.text = "";
            }).Subscribe();
        }

        #endregion

        #region Private

        private bool _isShowing;

        #endregion
    }
}