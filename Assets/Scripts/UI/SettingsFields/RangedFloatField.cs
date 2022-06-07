using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allow to show and edit float field in specific range. </summary>
    public class RangedFloatField : RangedSettingsField<float>
    {
        #region Editor fields

        [SerializeField] private Slider ValueSlider;
        [SerializeField] private TMP_Text ValueLabel;

        #endregion

        /// <inheritdoc />
        public override float Value => ValueSlider.value;

        /// <inheritdoc />
        public override IObservable<float> OnValueChanged()
        {
            return ValueSlider.OnValueChangedAsObservable().Do(v => ValueLabel.text = $"{v:F3}");
        }

        /// <inheritdoc />
        protected override void Setup(float defaultValue, float minValue, float maxValue)
        {
            ValueSlider.maxValue = maxValue;
            ValueSlider.minValue = minValue;
            ValueSlider.value = defaultValue;
            ValueLabel.text = $"{defaultValue}";
        }
    }
}