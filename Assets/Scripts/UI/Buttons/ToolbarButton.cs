using System;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Buttons
{
    [RequireComponent(typeof(Button), typeof(Tooltip))]
    public class ToolbarButton : MonoBehaviour
    {
        [SerializeField] private RawImage ImageLabel;
        [SerializeField] private TMP_Text TextLabel;

        public IObservable<Unit> OnClick => GetComponent<Button>().OnClickAsObservable();

        public void Setup(Texture2D logo, string text)
        {
            if (logo is null)
            {
                TextLabel.SetLocalizedText(text);
                ImageLabel.gameObject.SetActive(false);
            }
            else
            {
                ImageLabel.texture = logo;
                GetComponent<Tooltip>().TooltipText = text.tr();
                TextLabel.gameObject.SetActive(false);
            }
        }
    }
}