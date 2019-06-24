using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    [RequireComponent(typeof(Slider))]
    public class UITimelineSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<float> OnTimelineChanged;

        private event Action<float> OnTimelinePointerUp;
        private Slider m_slider;
        private void Start()
        {
            m_slider = gameObject.GetComponent<Slider>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var pointerUp = Observable.FromEvent<float>(add => OnTimelinePointerUp += add, remove => OnTimelinePointerUp -= remove);
            m_slider.ObserveEveryValueChanged(s => s.value)
                .Do(v => OnTimelineChanged?.Invoke(v))
                .TakeUntil(pointerUp)
                .Subscribe();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnTimelinePointerUp?.Invoke(m_slider.value);
        }
    }
}
