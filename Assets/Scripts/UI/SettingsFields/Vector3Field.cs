using System;
using System.Globalization;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    public class Vector3Field : SettingsField<Vector3>
    {
        [SerializeField] private TMP_InputField InputFieldX;
        [SerializeField] private TMP_InputField InputFieldY;
        [SerializeField] private TMP_InputField InputFieldZ;

        private readonly Subject<Vector3> _subject = new Subject<Vector3>();
        private Vector3 _value;

        public override Vector3 Value => _value;

        public override IObservable<Vector3> OnValueChanged() => _subject;

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
    }
}