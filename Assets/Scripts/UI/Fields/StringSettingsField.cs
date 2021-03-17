using System;
using System.Reflection;
using Elektronik.Settings;
using UniRx;
using UnityEngine.UI;

namespace Elektronik.UI.Fields
{
    public class StringSettingsField : SettingsField
    {
        public InputField Field;

        protected override void Start()
        {
            base.Start();
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