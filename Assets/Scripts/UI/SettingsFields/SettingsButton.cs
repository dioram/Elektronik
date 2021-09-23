using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    public class SettingsButton: SettingsFieldBase
    {
        [SerializeField] private Button Button;

        public IObservable<Unit> OnClick() => Button.OnClickAsObservable();

        public void Setup(string labelText, string tooltipText)
        {
            SetupText(labelText, tooltipText);
        }
    }
}