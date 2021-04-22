using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Slider))]
    public class TimelineSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<float> OnTimelineChanged;

        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }

        private event Action<float> OnTimelinePointerUp;
        private Slider _slider;

        private void Start()
        {
            _slider = gameObject.GetComponent<Slider>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var pointerUp =
                    Observable.FromEvent<float>(add => OnTimelinePointerUp += add,
                                                remove => OnTimelinePointerUp -= remove);
            _slider.ObserveEveryValueChanged(s => s.value)
                    .Do(v => OnTimelineChanged?.Invoke(v))
                    .TakeUntil(pointerUp)
                    .Subscribe();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnTimelinePointerUp?.Invoke(_slider.value);
        }
    }
}