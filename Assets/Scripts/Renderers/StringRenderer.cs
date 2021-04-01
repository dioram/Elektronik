using System;
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

        public bool IsShowing { get; private set; }

        public void Render(string data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
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
            Label.text = "";
        }

        #endregion

        #region Private

        [SerializeField] private TMP_Text Label;

        #endregion
    }
}