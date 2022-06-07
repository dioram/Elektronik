using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allow to show and edit one field from  <see cref="SettingsBag"/>. </summary>
    [RequireComponent(typeof(Tooltip))]
    public class SettingsFieldBase : MonoBehaviour
    {
        [SerializeField] private TMP_Text Label;

        /// <summary> Sets name and tooltip for this component. </summary>
        /// <param name="labelText"> Name of the field. </param>
        /// <param name="tooltipText"> Additional information about the field. </param>
        protected void SetupText(string labelText, string tooltipText)
        {
            Label.SetLocalizedText(labelText);
            GetComponent<Tooltip>().TooltipText = tooltipText.tr();
        }
    }
}