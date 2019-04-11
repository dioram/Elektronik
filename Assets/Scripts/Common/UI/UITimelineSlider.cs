using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    [RequireComponent(typeof(Slider))]
    public class UITimelineSlider : MonoBehaviour, IPointerClickHandler
    {
        public event Action<float> OnTimelineSliderClick;
        private Slider m_slider;
        private void Start()
        {
            m_slider = gameObject.GetComponent<Slider>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnTimelineSliderClick?.Invoke(m_slider.value);
        }
    }
}
