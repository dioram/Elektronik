using System.Linq;
using Elektronik.Common;
using Elektronik.Offline.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Elektronik.Offline
{
    [RequireComponent(typeof(FastPointCloud))]
    public class SlamEventsManager : MonoBehaviour
    {
        public Slider timelineSlider;
        public Text timelineLabel;

        ISlamEvent[] m_events;
        SlamPoint[][] m_backwardEventsPoints;
        SlamObservation[][] m_backwardEventsObservations;

        bool m_readyToWork = false;
        bool m_play = false;

        FastPointCloud m_fastPointCloud;
        int m_position = -1;

        void Start()
        {
            m_events = EventReader.AnalyzeFile(FileModeSettings.Path);
            m_backwardEventsPoints = new SlamPoint[m_events.Length][];
            m_backwardEventsObservations = new SlamObservation[m_events.Length][];
            m_fastPointCloud = GetComponent<FastPointCloud>();
            timelineSlider.minValue = 0;
            timelineSlider.maxValue = m_events.Length;
            
            StartCoroutine(ProcessingEventsCoroutine());

        }

        void UpdateTime()
        {
            if (m_position < 0 || m_position >= m_events.Length)
                return;
            timelineSlider.value = m_position;
            TimeSpan time = TimeSpan.FromMilliseconds(m_events[m_position].Timestamp);
            timelineLabel.text = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }

        void FixedUpdate()
        {
            if (m_play)
            {
                m_play = NextEvent(needRepaint: true);
                UpdateTime();
            }
        }

        public void SetPosition(int idx)
        {
            Pause();
            if (idx == m_position)
                return;
            else if (idx > m_position)
            {
                while (m_position != idx)
                {
                    NextEvent(needRepaint: false);
                }
            }
            else
            {
                while (m_position != idx)
                {
                    PrevEvent(needRepaint: false);
                }
            }
            m_fastPointCloud.Repaint();
            UpdateTime();
        }

        public void Forward()
        {
            if (m_readyToWork)
            {
                Pause();
                NextKeyEvent();
                UpdateTime();
            }
        }

        public void Backward()
        {
            if (m_readyToWork)
            {
                Pause();
                PrevKeyEvent();
                UpdateTime();
            }
        }

        public void Play()
        {
            if (m_readyToWork)
                m_play = true;
        }

        public void Pause()
        {
            m_play = false;
        }

        public void Stop()
        {
            m_play = false;
            m_fastPointCloud.Clear();
            ResetPosition();
        }

        bool NextKeyEvent()
        {
            int idxOfKeyEvent = Array.FindIndex(m_events, m_position + 1, e => e.IsKeyEvent);
            if (idxOfKeyEvent == -1)
                return false;
            while (m_position != idxOfKeyEvent)
            {
                NextEvent(needRepaint: false);
            }
            m_fastPointCloud.Repaint();
            return true;
        }

        bool PrevKeyEvent()
        {
            int idxOfKeyEvent = -1;
            for (int i = m_position - 1; i >= 0; --i)
            {
                if (m_events[i].IsKeyEvent)
                {
                    idxOfKeyEvent = i;
                    break;
                }
            }
            if (idxOfKeyEvent == -1)
                return false;
            while (m_position != idxOfKeyEvent)
            {
                PrevEvent(needRepaint: false);
            }
            m_fastPointCloud.Repaint();
            return true;
        }

        // Set need repaint for immediate update a point cloud, but note that it isn't efficiently
        bool NextEvent(bool needRepaint)
        {
            m_position += 1;
            if (m_position >= m_events.Length)
                return false;
            Debug.AssertFormat(m_position < m_events.Length, "Position is {0}, but count of events is {1}", m_position, m_events.Length);
            if (m_events[m_position].Points != null)
            {
                SlamPoint[] validPoints = m_events[m_position].Points.Where(p => p.id != -1).ToArray();
                for (int i = 0; i < validPoints.Length; ++i)
                {
                    Vector3 curPointPosition;
                    Color curPointColor;
                    m_fastPointCloud.GetPoint(i, out curPointPosition, out curPointColor);
                    m_fastPointCloud.SetPoint(validPoints[i].id, curPointPosition + validPoints[i].position, validPoints[i].color);
                }
                if (needRepaint)
                    m_fastPointCloud.Repaint();
            }
            // TODO: добавить граф Observations
            return true;
        }

        bool PrevEvent(bool needRepaint)
        {
            m_position -= 1;
            if (m_position <= -1)
                return false;
            if (m_backwardEventsPoints[m_position] != null)
            {
                int[] pointIds = m_backwardEventsPoints[m_position].Select(p => p.id).ToArray();
                Vector3[] pointsPositions = m_backwardEventsPoints[m_position].Select(p => p.position).ToArray();
                Color[] pointsColors = m_backwardEventsPoints[m_position].Select(p => p.color).ToArray();
                m_fastPointCloud.SetPoints(pointIds, pointsPositions, pointsColors);
                if (needRepaint)
                    m_fastPointCloud.Repaint();
            }
            // TODO: добавить граф Observations
            return true;
        }

        void ResetPosition()
        {
            m_position = -1;
        }

        IEnumerator ProcessingEventsCoroutine()
        {
            for (int eventIdx = 0; eventIdx < m_events.Length; ++eventIdx)
            {
                if (eventIdx % 10 == 0)
                {
                    Debug.Log(String.Format("Processed {0} events...", eventIdx));
                    yield return null;
                }
                ISlamEvent nextEvent = m_events[eventIdx];
                if (nextEvent.Points != null)
                {
                    SlamPoint[] nextEventPoints = nextEvent.Points;
                    SlamPoint[] currentPoints = new SlamPoint[nextEventPoints.Length];
                    for (int pointIdx = 0; pointIdx < currentPoints.Length; ++pointIdx)
                    {
                        currentPoints[pointIdx].id = nextEventPoints[pointIdx].id;
                        m_fastPointCloud.GetPoint(currentPoints[pointIdx].id, out currentPoints[pointIdx].position, out currentPoints[pointIdx].color);
                    }
                    m_backwardEventsPoints[eventIdx] = currentPoints;
                    NextEvent(needRepaint: false);
                }

                if (nextEvent.Observations != null)
                {
                    continue; // TODO: добавить граф Observations
                }
            }
            m_fastPointCloud.Clear();
            ResetPosition();
            m_readyToWork = true;
            yield return null;
        }

    }
}