using System;
using System.Reflection;
using Elektronik.Settings;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class SettingsField : MonoBehaviour
    {
        public SettingsBag SettingsBag;
        public string FieldToolTip;
        public string FieldName;
        public Type FieldType;

        public InputField Field;
        public Text Tooltip;

        private void Start()
        {
            Tooltip.text = FieldToolTip;
            if (IsDirectory())
            {
                GetComponent<InputWithBrowse>().folderMode = true;
            }

            Field.OnValueChangedAsObservable().Subscribe(OnTextChanged);
        }

        private bool IsDirectory()
        {
            var attr = SettingsBag.GetType().GetField(FieldName).GetCustomAttribute<PathAttribute>();
            return attr != null && attr.PathType == PathAttribute.PathTypes.Directory;
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