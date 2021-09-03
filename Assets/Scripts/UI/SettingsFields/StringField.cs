using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    public class StringField : SettingsField<string>
    {
        [SerializeField] private TMP_InputField InputField;

        public override string Value => InputField.text;

        public override IObservable<string> OnValueChanged() => InputField.OnValueChangedAsObservable();

        protected override void Setup(string defaultValue)
        {
            InputField.text = defaultValue;
        }
    }
}