using System;
using TMPro;
using UniRx;
using UnityEngine;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allows to show and edit string fields from  <see cref="SettingsBag"/>. </summary>
    public class StringField : SettingsField<string>
    {
        [SerializeField] private TMP_InputField InputField;

        /// <inheritdoc />
        public override string Value => InputField.text;

        /// <inheritdoc />
        public override IObservable<string> OnValueChanged() => InputField.OnValueChangedAsObservable();

        /// <inheritdoc />
        protected override void Setup(string defaultValue)
        {
            InputField.text = defaultValue;
        }
    }
}