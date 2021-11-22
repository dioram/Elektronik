using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allow to show and edit integer field in specific range. </summary>
    public class RangedIntegerField: RangedSettingsField<int>
    {
        #region Private

        [SerializeField] private Slider ValueSlider;
        [SerializeField] private TMP_Text ValueLabel;
        
        #endregion

        /// <inheritdoc />
        public override int Value => (int)ValueSlider.value;

        /// <inheritdoc />
        public override IObservable<int> OnValueChanged()
        {
            return ValueSlider.OnValueChangedAsObservable()
                    .Do(v => ValueLabel.text = $"{v}")
                    .Select(v => (int)v);
        }

        /// <inheritdoc />
        protected override void Setup(int defaultValue, int minValue, int maxValue)
        {
            ValueSlider.maxValue = maxValue;
            ValueSlider.minValue = minValue;
            ValueSlider.value = defaultValue;
            ValueLabel.text = $"{defaultValue}";
        }
    }
}