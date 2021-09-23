using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    public class RangedIntegerField: RangedSettingsField<int>
    {
        [SerializeField] private Slider ValueSlider;
        [SerializeField] private TMP_Text ValueLabel;

        public override int Value => (int)ValueSlider.value;
        public override IObservable<int> OnValueChanged()
        {
            return ValueSlider.OnValueChangedAsObservable()
                    .Do(v => ValueLabel.text = $"{v}")
                    .Select(v => (int)v);
        }

        protected override void Setup(int defaultValue, int minValue, int maxValue)
        {
            ValueSlider.maxValue = maxValue;
            ValueSlider.minValue = minValue;
            ValueSlider.value = defaultValue;
            ValueLabel.text = $"{defaultValue}";
        }
    }
}