using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allows to call action defined in  <see cref="SettingsBag"/>. </summary>
    public class SettingsButton : SettingsFieldBase
    {
        [SerializeField] private Button Button;

        /// <summary> RX observable for click events. </summary>
        /// <returns></returns>
        public IObservable<Unit> OnClick() => Button.OnClickAsObservable();

        /// <summary> This function sets up all visual elements of the field. </summary>
        /// <param name="labelText"> Name of the field. </param>
        /// <param name="tooltipText"> Additional information about the field. </param>
        public void Setup(string labelText, string tooltipText)
        {
            SetupText(labelText, tooltipText);
        }
    }
}