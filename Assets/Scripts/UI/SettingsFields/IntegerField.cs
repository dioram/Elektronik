using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary> UI component that allows to show and edit integer field from  <see cref="SettingsBag"/>. </summary>
    public class IntegerField : SettingsField<int>
    {
        #region Editor fields

        [SerializeField] private TMP_InputField InputField;
        [SerializeField] private Button Increase;
        [SerializeField] private Button Decrease;

        #endregion

        /// <inheritdoc />
        public override int Value => int.Parse(InputField.text);

        /// <inheritdoc />
        public override IObservable<int> OnValueChanged() => InputField.OnValueChangedAsObservable().Select(int.Parse);

        /// <inheritdoc />
        protected override void Setup(int defaultValue)
        {
            InputField.text = $"{defaultValue}";
        }

        #region Unity events

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

        #endregion
    }
}