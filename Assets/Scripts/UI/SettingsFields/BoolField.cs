using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    public class BoolField: SettingsField<bool>
    {
        [SerializeField] private Toggle CheckBox;

        public override bool Value => CheckBox.isOn;

        public override IObservable<bool> OnValueChanged() => CheckBox.OnValueChangedAsObservable();

        protected override void Setup(bool defaultValue)
        {
            CheckBox.isOn = defaultValue;
        }
    }
}