using System;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary>
    /// UI component that allow to show and edit one field from  <see cref="SettingsBag"/> in specific range.
    /// </summary>
    public abstract class RangedSettingsField<TFieldType> : SettingsFieldBase
    {
        /// <summary> Current value of this field. </summary>
        public abstract TFieldType Value { get; }

        public abstract IObservable<TFieldType> OnValueChanged();

        /// <summary> This function sets up all visual elements of the field. </summary>
        /// <param name="labelText"> Name of the field. </param>
        /// <param name="tooltipText"> Additional information about the field. </param>
        /// <param name="defaultValue"> Initial value of the field. </param>
        /// <param name="minValue"> Minimal possible value of the field. </param>
        /// <param name="maxValue"> Maximum possible value of the field. </param>
        public void Setup(string labelText, string tooltipText, TFieldType defaultValue, TFieldType minValue,
                          TFieldType maxValue)
        {
            SetupText(labelText, tooltipText);
            Setup(defaultValue, minValue, maxValue);
        }

        /// <summary> This function sets up default value of the field. </summary>
        /// <param name="defaultValue"> Initial value of the field. </param>
        /// <param name="minValue"> Minimal possible value of the field. </param>
        /// <param name="maxValue"> Maximum possible value of the field. </param>
        protected abstract void Setup(TFieldType defaultValue, TFieldType minValue, TFieldType maxValue);
    }
}