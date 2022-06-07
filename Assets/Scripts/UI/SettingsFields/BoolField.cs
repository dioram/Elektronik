using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allows to show and edit boolean fields from  <see cref="SettingsBag"/>. </summary>
    public class BoolField : SettingsField<bool>
    {
        [SerializeField] private Toggle CheckBox;

        /// <inheritdoc />
        public override bool Value => CheckBox.isOn;

        /// <inheritdoc />
        public override IObservable<bool> OnValueChanged() => CheckBox.OnValueChangedAsObservable();

        /// <inheritdoc />
        protected override void Setup(bool defaultValue)
        {
            CheckBox.isOn = defaultValue;
        }
    }
}