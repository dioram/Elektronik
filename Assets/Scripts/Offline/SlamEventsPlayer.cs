using UnityEngine;
using System.Collections;
using Elektronik.Common.SlamEventsCommandPattern;
using UnityEngine.UI;
using Elektronik.Common;
using System;
using Elektronik.Common.Events;
using Elektronik.Common.Data;
using Elektronik.Common.UI;
using System.Linq;
using System.Collections.Generic;

namespace Elektronik.Offline
{
    public class SlamEventsPlayer : MonoBehaviour
    {
        private bool m_play = false;
        public Slider timelineSlider;
        public Text timelineLabel;

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
            if (eventsManager.ReadyToPlay)
            {
                if (m_play)
                {
                    m_play = eventsManager.Next();
                }
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    PrevKey();
                }
                if (Input.GetKeyDown(KeyCode.RightBracket))
                {
                    NextKey();
                }
                eventsManager.UpdateEventInfo();
                UpdateTime();
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