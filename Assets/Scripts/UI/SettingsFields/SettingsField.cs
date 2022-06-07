using System;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allow to show and edit one field from  <see cref="SettingsBag"/>. </summary>
    /// <typeparam name="TFieldType"> Type of data represented by this field. </typeparam>
    public abstract class SettingsField<TFieldType> : SettingsFieldBase
    {
        /// <summary> Current value of this field. </summary>
        public abstract TFieldType Value { get; }

        /// <summary> This function sets up all visual elements of the field. </summary>
        /// <param name="labelText"> Name of the field. </param>
        /// <param name="tooltipText"> Additional information about the field. </param>
        /// <param name="defaultValue"> Initial value of the field. </param>
        public void Setup(string labelText, string tooltipText, TFieldType defaultValue)
        {
            SetupText(labelText, tooltipText);
            Setup(defaultValue);
        }

        /// <summary> RX observable for value changes. </summary>
        public abstract IObservable<TFieldType> OnValueChanged();

        /// <summary> This function sets up default value of the field. </summary>
        /// <param name="defaultValue"> Initial value of the field. </param>
        protected abstract void Setup(TFieldType defaultValue);
    }
}