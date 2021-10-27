using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SettingsFields
{
    public class IntegerField: SettingsField<int>   
    {
        [SerializeField] private TMP_InputField InputField;
        [SerializeField] private Button Increase;
        [SerializeField] private Button Decrease;

        public override int Value => int.Parse(InputField.text);

        public override IObservable<int> OnValueChanged() => InputField.OnValueChangedAsObservable().Select(int.Parse);

        protected override void Setup(int defaultValue)
        {
            InputField.text = $"{defaultValue}";
        }

        protected void Awake()
        {
            Increase.OnClickAsObservable()
                    .Select(_ => int.Parse(InputField.text))
                    .Subscribe(i => InputField.text = $"{i + 1}")
                    .AddTo(this);
            Decrease.OnClickAsObservable()
                    .Select(_ => int.Parse(InputField.text))
                    .Subscribe(i => InputField.text = $"{i - 1}")
                    .AddTo(this);
        }
    }
}