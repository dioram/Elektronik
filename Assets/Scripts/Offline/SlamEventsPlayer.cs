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
        public Camera mainCamera;
        public OrbitalCameraForPointInSpace cameraAroundPoint;
        public Slider timelineSlider;
        public Text timelineLabel;

        public SpecialInfoListBoxItem specialInfoPrefabItem;

        private Text m_eventText;
        private UIListBox m_specialInformationListBox;

        public SlamEventsManager eventsManager;

        private void Awake()
        {
            m_eventText = GameObject
                .Find(@"Common/NonVRMode/UIControls/Common info/[SW] Events logger background/Viewport/Content/Events logger background")
                .GetComponentInChildren<Text>();
            m_specialInformationListBox = GameObject
                .Find(@"Common/NonVRMode/UIControls/Common info/Special information")
                .GetComponent<UIListBox>();
        }

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

        private void UpdateSpecialObjectsUI()
        {
            m_specialInformationListBox.Clear();

            var observations = eventsManager.GetCurrentEvent().Observations;
            if (observations != null)
            {
                IEnumerable<SlamObservation> specialObservations = observations.Where(obs => obs.message != null);
                foreach (var obs in specialObservations)
                {
                    specialInfoPrefabItem.Observation = obs;
                    m_specialInformationListBox.Add(specialInfoPrefabItem);
                    m_specialInformationListBox.OnSelectionChanged += OnSpecialInfoClicked;
                }
            }

            var pts = eventsManager.GetCurrentEvent().Points;
            if (pts != null)
            {
                IEnumerable<SlamPoint> specialPts = pts.Where(pt => pt.message != null);
                foreach (var pt in specialPts)
                {
                    specialInfoPrefabItem.Point = pt;
                    m_specialInformationListBox.Add(specialInfoPrefabItem);
                }
            }
        }

        private void OnSpecialInfoClicked(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            var item = m_specialInformationListBox[e.index] as SpecialInfoListBoxItem;
            Vector3 to = new Vector3();
            Quaternion toOrientation = new Quaternion();
            if (item.Point != null)
            {
                toOrientation = Quaternion.FromToRotation(mainCamera.transform.position, item.Point.Value.position);
                to = item.Point.Value.position;
            }
            else if (item.Observation != null)
            {
                toOrientation = Quaternion.FromToRotation(mainCamera.transform.position, item.Observation.position);
                to = item.Observation.position;
            }

            mainCamera.gameObject.SetActive(false);
            cameraAroundPoint.FlyToPosition(mainCamera.transform.position, to, toOrientation);
            cameraAroundPoint.OnSwitchOff += () => mainCamera.gameObject.SetActive(true);
        }

        private void UpdateEventText(Package @event)
        {
            Package currentEvent = eventsManager.GetCurrentEvent();
            if (currentEvent != null)
            {
                m_eventText.text = currentEvent.Summary();
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
                UpdateTime();
                UpdateEventText(eventsManager.GetCurrentEvent());
                UpdateSpecialObjectsUI();
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