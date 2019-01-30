using UnityEngine;
using System.Collections;
using Elektronik.Common.SlamEventsCommandPattern;
using UnityEngine.UI;
using Elektronik.Common;
using System;
using Elektronik.Common.Events;
using Elektronik.Common.Data;

namespace Elektronik.Offline
{
    public class SlamEventsPlayer : MonoBehaviour
    {
        private bool m_play = false;

        public Slider timelineSlider;
        public Text timelineLabel;
        public Text eventText;

        public SlamEventsManager eventsManager;

        private void Start()
        {
            StartCoroutine(WaitForManagerLengthParameter());
        }

        IEnumerator WaitForManagerLengthParameter()
        {
            while (!eventsManager.ReadyToPlay)
            {
                yield return new WaitForSeconds(1);
            }

            timelineSlider.minValue = 0;
            timelineSlider.maxValue = eventsManager.GetLength() - 1;

            yield return null;
        }

        private void UpdateEventText(Package @event)
        {
            Package currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null)
            {
                eventText.text = currentEvent.Summary();
            }
        }

        private void UpdateTime()
        {
            Package currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null && currentEvent.Timestamp != -1)
            {
                DateTime timestamp = new DateTime();
                timestamp += TimeSpan.FromMilliseconds(currentEvent.Timestamp);
                timelineLabel.text = timestamp.ToString("hh:mm:ss.fff");
                timelineSlider.value = eventsManager.GetCurrentEventPosition();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                PrevKey();
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                NextKey();
            }
            if (eventsManager.ReadyToPlay)
            {
                UpdateTime();
                UpdateEventText(eventsManager.GetCurrentEvent());
            }
        }

        void FixedUpdate()
        {
            if (m_play)
            {
                m_play = eventsManager.Next();
            }
        }

        public void Play()
        {
            if (eventsManager.ReadyToPlay)
            {
                m_play = true;
            }
        }

        public void Pause()
        {
            m_play = false;
        }

        public void Stop()
        {
            m_play = false;
            eventsManager.Clear();
        }

        public void PrevKey()
        {
            Pause();
            eventsManager.PrevKeyEvent();
        }

        public void NextKey()
        {
            Pause();
            eventsManager.NextKeyEvent();
        }

        public void SetPosition(float i)
        {
            if (Input.GetMouseButton(0))
            {
                Pause();
                eventsManager.SetPosition((int)Math.Floor(i));
            }
        }
    }
}