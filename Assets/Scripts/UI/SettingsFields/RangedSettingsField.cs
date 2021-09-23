using System;

namespace Elektronik.UI.SettingsFields
{
    public abstract class RangedSettingsField<TFieldType> : SettingsFieldBase
    {
        public abstract TFieldType Value { get; }

        public abstract IObservable<TFieldType> OnValueChanged();

        public void Setup(string labelText, string tooltipText, TFieldType defaultValue,
                          TFieldType minValue, TFieldType maxValue)
        {
            SetupText(labelText, tooltipText);
            Setup(defaultValue, minValue, maxValue);
        }

        protected abstract void Setup(TFieldType defaultValue, TFieldType minValue,
                                                         TFieldType maxValue);
    }
}