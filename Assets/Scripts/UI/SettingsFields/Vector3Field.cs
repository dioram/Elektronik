using System;
using System.Globalization;
using TMPro;
using UniRx;
using UnityEngine;
using Elektronik.Settings;

namespace Elektronik.UI.SettingsFields
{
    /// <summary>
    /// UI component that allows to show and edit <c>UnityEngine.Vector3</c> fields from <see cref="SettingsBag"/>.
    /// </summary>
    public class Vector3Field : SettingsField<Vector3>
    {
        #region Editor fields

        [SerializeField] private TMP_InputField InputFieldX;
        [SerializeField] private TMP_InputField InputFieldY;
        [SerializeField] private TMP_InputField InputFieldZ;

        #endregion

        /// <inheritdoc />
        public override Vector3 Value => _value;

        /// <inheritdoc />
        public override IObservable<Vector3> OnValueChanged() => _subject;

        /// <inheritdoc />
        protected override void Setup(Vector3 defaultValue)
        {
            _value = defaultValue;
            InputFieldX.text = defaultValue.x.ToString(CultureInfo.InvariantCulture);
            InputFieldY.text = defaultValue.y.ToString(CultureInfo.InvariantCulture);
            InputFieldZ.text = defaultValue.z.ToString(CultureInfo.InvariantCulture);
            InputFieldX.OnValueChangedAsObservable()
                    .Select(_ => (int.TryParse(InputFieldX.text, out var res), res))
                    .Where(pair => pair.Item1)
                    .Do(pair => _value = new Vector3(pair.res, Value.y, Value.z))
                    .Select(_ => _value)
                    .Subscribe(_subject)
                    .AddTo(this);
            InputFieldY.OnValueChangedAsObservable()
                    .Select(_ => (int.TryParse(InputFieldY.text, out var res), res))
                    .Where(pair => pair.Item1)
                    .Do(pair => _value = new Vector3(Value.x, pair.res, Value.z))
                    .Select(_ => _value)
                    .Subscribe(_subject)
                    .AddTo(this);
            InputFieldZ.OnValueChangedAsObservable()
                    .Select(_ => (int.TryParse(InputFieldZ.text, out var res), res))
                    .Where(pair => pair.Item1)
                    .Do(pair => _value = new Vector3(Value.x, Value.y, pair.res))
                    .Select(_ => _value)
                    .Subscribe(_subject)
                    .AddTo(this);
        }

        #region Private

        private readonly Subject<Vector3> _subject = new Subject<Vector3>();
        private Vector3 _value;

        #endregion
    }
}