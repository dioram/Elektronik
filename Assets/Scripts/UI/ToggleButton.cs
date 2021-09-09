using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Color BaseColor;
        [SerializeField] private Color ToggledColor;
        [SerializeField] private bool IsToggled = false;
        [SerializeField] private TMP_Text TextLabel;
        private Button _button;
        private Image _image;

        public string Text => TextLabel.text;

        public IObservable<(ToggleButton button, bool isToggled)> OnToggled { get; private set; }

        public void SetToggled(bool isToggled)
        {
            IsToggled = isToggled;
            if (_image != null) _image.color = IsToggled ? ToggledColor : BaseColor;
        }

        public void Setup(string text)
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _image.color = IsToggled ? ToggledColor : BaseColor;
            TextLabel.text = text;

            var subject = new Subject<(ToggleButton, bool)>();
            _button.OnClickAsObservable()
                    .Do(_ => IsToggled = !IsToggled)
                    .Do(_ => _image.color = IsToggled ? ToggledColor : BaseColor)
                    .Select(_ => (this, IsToggled))
                    .Subscribe(subject)
                    .AddTo(this);
            OnToggled = subject; 
        }
    }
}