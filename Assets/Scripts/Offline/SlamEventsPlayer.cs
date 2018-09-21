using UnityEngine;
using System.Collections;
using Elektronik.Offline.SlamEventsCommandPattern;
using UnityEngine.UI;
using Elektronik.Common;
using System;
using Elektronik.Offline.Events;

namespace Elektronik.Offline
{
    public class SlamEventsPlayer : MonoBehaviour
    {
        private bool m_play = false;

        public Slider timelineSlider;
        public Text timelineLabel;
        public ListView loggerListView;

        public SlamEventsManager eventsManager;

        private void Start()
        {
            timelineSlider.minValue = 0;
            timelineSlider.maxValue = eventsManager.GetLength();
        }

        private void PushLog()
        {
            ISlamEvent currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null && currentEvent.IsKeyEvent)
                loggerListView.PushItem(currentEvent.ToString());
        }

        private void PopLog()
        {
            ISlamEvent currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null && currentEvent.IsKeyEvent)
                loggerListView.PopItem();
        }

        private void UpdateTime()
        {
            ISlamEvent currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null)
            {
                timelineLabel.text = TimeSpan.FromMilliseconds(eventsManager.GetCurrentEvent().Timestamp).ToString();
                timelineSlider.value = eventsManager.GetCurrentEventPosition();
            }
        }

        void FixedUpdate()
        {
            if (m_play)
            {
                m_play = eventsManager.Next();
                if (m_play)
                {
                    PushLog();
                    UpdateTime();
                }
            }
        }

        public void Play()
        {
            m_play = true;
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
            if (eventsManager.PrevKeyEvent())
            {
                UpdateTime();
                PopLog();
            }
            
        }

        public void NextKey()
        {
            Pause();
            if (eventsManager.NextKeyEvent())
            {
                UpdateTime();
                PushLog();
            }
        }
    }
}