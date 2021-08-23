using System;
using System.Globalization;
using Elektronik.Cameras;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class Scale : MonoBehaviour
    {
        [SerializeField] private Slider Slider;
        [SerializeField] private TMP_InputField InputField;
        [SerializeField] private Button SubmitButton;
        [SerializeField] private GameObject Renderers;
        [SerializeField] private LookableCamera Camera;
        
        public event Action<float> OnScaleChanged;
        
        private void Start()
        {
            foreach (var renderer in Renderers.GetComponentsInChildren<ISourceRenderer>())
            {
                OnScaleChanged += renderer.SetScale;
            }
            Slider.OnValueChangedAsObservable()
                  .Subscribe(v => InputField.text = v.ToString(CultureInfo.InvariantCulture));
            Slider.value = 1;
            SubmitButton.OnClickAsObservable()
                        .Select(_ => InputField.text)
                        .Select(s => (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v), v))
                        .Where(r => r.Item1)
                        .Select(r => r.v)
                        .Do(v => OnScaleChanged?.Invoke(v))
                        .Where(v => Slider.minValue <= v && v <= Slider.maxValue)
                        .Do(v => Slider.value = v)
                        .Do(v => Camera.Scale = v)
                        .Subscribe();
        }
    }
}