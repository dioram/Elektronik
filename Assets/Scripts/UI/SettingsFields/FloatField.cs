using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allows to show and edit float field from  <see cref="SettingsBag"/>. </summary>
    public class FloatField : SettingsField<float>
    {
        #region Editor fields

        [SerializeField] private TMP_InputField InputField;
        [SerializeField] private Button Increase;
        [SerializeField] private Button Decrease;

        #endregion

        /// <inheritdoc />
        public override float Value => float.Parse(InputField.text);

        /// <inheritdoc />
        public override IObservable<float> OnValueChanged()
        {
            return InputField.OnValueChangedAsObservable().Select(float.Parse);
        }

        /// <inheritdoc />
        protected override void Setup(float defaultValue)
        {
            InputField.text = $"{defaultValue}";
        }

        #region Unity events

        private void Awake()
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

        #endregion
    }
}