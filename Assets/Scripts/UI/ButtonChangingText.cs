using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    public class ButtonChangingText : ChangingButton
    {
        public string[] Texts;
        public TMP_Text TargetTMPText;
        public Text TargetText;

        protected override void Start()
        {
            MaxState = Texts.Length;
            base.Start();
        }

        protected override void SetValue()
        {
            if (TargetTMPText != null) TargetTMPText.SetLocalizedText(Texts[State]);
            if (TargetText != null) TargetText.SetLocalizedText(Texts[State]);
        }
    }
}