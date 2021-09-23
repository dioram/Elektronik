using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    public class RangedFloatField: RangedSettingsField<float>
    {
        [SerializeField] private Slider ValueSlider;
        [SerializeField] private TMP_Text ValueLabel;

        public override float Value => ValueSlider.value;
        public override IObservable<float> OnValueChanged()
        {
            return ValueSlider.OnValueChangedAsObservable().Do(v => ValueLabel.text = $"{v:F3}");
        }

        protected override void Setup(float defaultValue, float minValue, float maxValue)
        {
            ValueSlider.maxValue = maxValue;
            ValueSlider.minValue = minValue;
            ValueSlider.value = defaultValue;
            ValueLabel.text = $"{defaultValue}";
        }
    }
}