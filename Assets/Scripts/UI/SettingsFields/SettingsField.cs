using System;

namespace Elektronik.UI.SettingsFields
{
    public abstract class SettingsField<TFieldType> : SettingsFieldBase
    {
        public abstract TFieldType Value { get; }
        
        public void Setup(string labelText, string tooltipText, TFieldType defaultValue)
        {
            SetupText(labelText, tooltipText);
            Setup(defaultValue);
        }

        public abstract IObservable<TFieldType> OnValueChanged();

        protected abstract void Setup(TFieldType defaultValue);
    }
}