using System;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allows to show and edit path to file or folder. </summary>
    [RequireComponent(typeof(InputWithBrowse))]
    public class PathField : SettingsFieldBase
    {
        [SerializeField] private TMP_InputField InputField;

        /// <summary> Current value of this field. </summary>
        public string Value => InputField.text;

        /// <summary> RX observable for value changes. </summary>
        public IObservable<string> OnValueChanged() => InputField.OnValueChangedAsObservable();

        /// <summary> This function sets up all visual elements of the field. </summary>
        /// <param name="labelText"> Name of the field. </param>
        /// <param name="tooltipText"> Additional information about the field. </param>
        /// <param name="defaultValue"> Initial value of the field. </param>
        /// <param name="folderMode"> true if it should look for folders, false - for files. </param>
        /// <param name="extensions"> Extensions that should be filtered. </param>
        public void Setup(string labelText, string tooltipText, string defaultValue, bool folderMode,
                          string[] extensions)
        {
            SetupText(labelText, tooltipText);
            InputField.text = defaultValue;
            var inputWithBrowse = GetComponent<InputWithBrowse>();
            inputWithBrowse.Title = labelText.tr();
            inputWithBrowse.FolderMode = folderMode;
            inputWithBrowse.Filters = extensions;
        }
    }
}