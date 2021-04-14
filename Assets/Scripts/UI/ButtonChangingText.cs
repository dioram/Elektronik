using System;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    public class ButtonChangingText : MonoBehaviour
    {
        public string[] Texts;
        public TMP_Text TargetTMPText;
        public Text TargetText;

        public int State
        {
            get => _state;
            set
            {
                _state = value % Texts.Length;
                SetText(_state);
            }
        }

        public IObservable<Unit> OnClickAsObservable() => _button.OnClickAsObservable();

        private Button _button;
        private int _state;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.OnClickAsObservable().Subscribe(_ => State++);
            SetText(0);
        }

        private void SetText(int state)
        {
            if (TargetTMPText != null) TargetTMPText.SetLocalizedText(Texts[state]);
            if (TargetText != null) TargetText.SetLocalizedText(Texts[state]);
        }
    }
}