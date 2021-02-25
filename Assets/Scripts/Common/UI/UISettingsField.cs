using System;
using Elektronik.Common.Settings;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class UISettingsField : MonoBehaviour
    {
        public SettingsBag SettingsBag;
        public string FieldToolTip;
        public string FieldName;
        public string FieldText;
        public Type FieldType;

        public InputField Field;
        public Text Tooltip;

        private void Start()
        {
            Tooltip.text = FieldToolTip;
            Field.text = FieldText;
            Field.OnValueChangedAsObservable().Do(OnTextChanged).Subscribe();
        }

        private void OnTextChanged(string text)
        {
            if (FieldType == typeof(int))
            {
                if (int.TryParse(text, out int val))
                {
                    SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, val);
                }
            }
            else if (FieldType == typeof(float))
            {
                if (float.TryParse(text, out float val))
                {
                    SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, val);
                }
            }
            else if (FieldType == typeof(string))
            {
                SettingsBag.GetType().GetField(FieldName).SetValue(SettingsBag, text);
            }
            else
            {
                throw new NotSupportedException("Other field types not implemented yet");
            }
        }
    }
}