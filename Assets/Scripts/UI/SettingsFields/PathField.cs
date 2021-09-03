using System;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    [RequireComponent(typeof(InputWithBrowse))]
    public class PathField : SettingsFieldBase
    {
        [SerializeField] private TMP_InputField InputField;

        public string Value => InputField.text;
        public IObservable<string> OnValueChanged() => InputField.OnValueChangedAsObservable();

        public void Setup(string labelText, string tooltipText, string defaultValue, bool folderMode,
                          string[] extensions)
        {
            SetupText(labelText, tooltipText);
            InputField.text = defaultValue;
            var inputWithBrowse = GetComponent<InputWithBrowse>();
            inputWithBrowse.Title = TextLocalizationExtender.GetLocalizedString(labelText);
            inputWithBrowse.FolderMode = folderMode;
            inputWithBrowse.Filters = extensions;
        }
    }
}