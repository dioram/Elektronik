using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    public class FloatField: SettingsField<float>
    {
        [SerializeField] private TMP_InputField InputField;
        [SerializeField] private Button Increase;
        [SerializeField] private Button Decrease;

        public override float Value => float.Parse(InputField.text);

        public override IObservable<float> OnValueChanged()
        {
            return InputField.OnValueChangedAsObservable().Select(float.Parse);
        }

        protected override void Setup(float defaultValue)
        {
            InputField.text = $"{defaultValue}";
        }

        protected void Awake()
        {
            Increase.OnClickAsObservable()
                    .Select(_ => float.Parse(InputField.text))
                    .Subscribe(i => InputField.text = $"{i + 0.01f}")
                    .AddTo(this);
            Decrease.OnClickAsObservable()
                    .Select(_ => float.Parse(InputField.text))
                    .Subscribe(i => InputField.text = $"{i - 0.01f}")
                    .AddTo(this);
        }
    }
}