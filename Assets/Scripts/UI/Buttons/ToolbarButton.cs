using System;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Buttons
{
    /// <summary> Button fro toolbar menu. </summary>
    [RequireComponent(typeof(Button), typeof(Tooltip))]
    public class ToolbarButton : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private RawImage ImageLabel;
        [SerializeField] private TMP_Text TextLabel;

        #endregion

        public IObservable<Unit> OnClickAsObservable => GetComponent<Button>().OnClickAsObservable();

        /// <summary> Setups button with given UI elements. </summary>
        public void Setup(Texture2D logo, string toolTipText)
        {
            if (logo is null)
            {
                TextLabel.SetLocalizedText(toolTipText);
                ImageLabel.gameObject.SetActive(false);
            }
            else
            {
                ImageLabel.texture = logo;
                GetComponent<Tooltip>().TooltipText = toolTipText.tr();
                TextLabel.gameObject.SetActive(false);
            }
        }
    }
}