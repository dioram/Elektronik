using System;
using Elektronik.Threading;
using TMPro;
using UnityEngine;

namespace Elektronik.Renderers
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
                MainThreadInvoker.Enqueue(() => gameObject.SetActive(_isShowing));
            }
        }

        public void Render(string data)
        {
            MainThreadInvoker.Enqueue(() =>
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
            });
        }

        public void Clear()
        {
            MainThreadInvoker.Enqueue(() =>
            {
                if (Label != null) Label.text = "";
            });
        }

        #endregion

        #region Private

        [SerializeField] private TMP_Text Label;
        private bool _isShowing;

        #endregion
    }
}