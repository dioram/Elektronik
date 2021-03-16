﻿using Elektronik.Common.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline.UI
{
    public class SlamEventsPlayer : MonoBehaviour
    {
        private bool _play = false;

        public UITimelineSlider timelineSlider;
        public Text timelineLabel;
        public SlamEventsManager eventsManager;

        private void Start()
        {
            timelineSlider.OnTimelineChanged += OnTimelineChanged;
            StartCoroutine(WaitForManagerLengthParameter());
        }

        private IEnumerator WaitForManagerLengthParameter()
        {
            while (!eventsManager.ReadyToPlay)
            {
                yield return new WaitForSeconds(1);
            }

            timelineSlider.GetComponent<Slider>().minValue = 0;
            timelineSlider.GetComponent<Slider>().maxValue = eventsManager.GetLength() - 1;

            yield return null;
        }

        private void UpdateTime()
        {
            var currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null && currentEvent.Timestamp != -1)
            {
                timelineLabel.text = TimeSpan.FromMilliseconds(currentEvent.Timestamp).ToString(@"mm\:ss\.fff");
                timelineSlider.GetComponent<Slider>().value = eventsManager.GetCurrentEventPosition();
            }
        }

        private void Update()
        {
            if (eventsManager.ReadyToPlay)
            {
                if (_play)
                {
                    _play = eventsManager.Next();
                    UpdateTime();
                }
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    PrevKey();
                    UpdateTime();
                }
                if (Input.GetKeyDown(KeyCode.RightBracket))
                {
                    NextKey();
                    UpdateTime();
                }
            }
        }

        public void Play()
        {
            if (eventsManager.ReadyToPlay)
            {
                _play = true;
            }
        }

        public void Pause()
        {
            _play = false;
        }

        public void Stop()
        {
            _play = false;
            eventsManager.Clear();
            UpdateTime();
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
            eventsManager.SetPosition((int)Math.Floor(i), () =>
            {
                UpdateTime();
            });
        }

        public void OnTimelineChanged(float i)
        {
            Pause();
            SetPosition(i);
        }
    }
}