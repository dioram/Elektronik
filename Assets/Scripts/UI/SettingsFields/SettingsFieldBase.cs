using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    [RequireComponent(typeof(Tooltip))]
    public class SettingsFieldBase: MonoBehaviour
    {
        [SerializeField] private TMP_Text Label;
        
        protected void SetupText(string labelText, string tooltipText)
        {
            Label.SetLocalizedText(labelText);
            GetComponent<Tooltip>().TooltipText = tooltipText.tr();
        }
    }
}