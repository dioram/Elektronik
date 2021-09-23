using Elektronik.UI.Localization;
using TMPro;
using UnityEngine.UI;

namespace Elektronik.UI.Buttons
{
    public class ButtonChangingText : ChangingButton
    {
        public string[] Texts;
        public TMP_Text TargetTMPText;
        public Text TargetText;

        protected override void Awake()
        {
            base.Awake();
            MaxState = Texts.Length;
        }

        protected override void SetValue()
        {
            if (TargetTMPText != null) TargetTMPText.SetLocalizedText(Texts[State]);
            if (TargetText != null) TargetText.SetLocalizedText(Texts[State]);
        }
    }
}