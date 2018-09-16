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
    [RequireComponent(typeof(FastPointCloud), typeof(SlamObservationsGraph))]
    public class SlamEventsManager : MonoBehaviour
    {
        public float scale;

        public Slider timelineSlider;
        public Text timelineLabel;
        public ListView loggerListView;

        ISlamEvent[] m_events;
        SlamPoint[][] m_backwardEventsPoints;
        SlamObservation[][] m_backwardEventsObservations;

        bool m_readyToWork = false;
        bool m_play = false;

        SlamObservationsGraph m_observationsGraph;
        FastPointCloud m_fastPointCloud;
        int m_position = -1;

        void Start()
        {
            ISlamEventDataConverter converter = new Camera2Unity3dSlamEventConverter(Matrix4x4.Scale(Vector3.one * scale));
            m_events = EventReader.AnalyzeFile(FileModeSettings.Path, converter);
            m_backwardEventsPoints = new SlamPoint[m_events.Length][];
            m_backwardEventsObservations = new SlamObservation[m_events.Length][];
            m_fastPointCloud = GetComponent<FastPointCloud>();
            m_observationsGraph = GetComponent<SlamObservationsGraph>();
            timelineSlider.minValue = 0;
            timelineSlider.maxValue = m_events.Length - 1;
            
            StartCoroutine(ProcessingEventsCoroutine()); // TODO: реализовать паттерн команда
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
            m_observationsGraph.Clear();
            m_fastPointCloud.Clear();
            loggerListView.Clear();
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
            m_observationsGraph.Repaint();
            return true;
        }

        bool PrevKeyEvent()
        {
            int idxOfKeyEvent = -1;
            if (m_position == m_events.Length)
                PrevEvent(needRepaint: false);
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

            // TODO: убрать костыль
            PrevEvent(needRepaint: false);
            NextEvent(needRepaint: false);
            m_fastPointCloud.Repaint();
            m_observationsGraph.Repaint();
            return true;
        }

        // Set need repaint for immediate update a point cloud, but note that it isn't efficiently
        bool NextEvent(bool needRepaint)
        {
            m_position += 1;
            if (m_position >= m_events.Length)
                return false;
            Debug.AssertFormat(m_position < m_events.Length, "Position is {0}, but count of events is {1}", m_position, m_events.Length);

            if (m_position > 0 && m_events[m_position - 1].Points != null)
            {
                SlamPoint[] validPoints = m_events[m_position - 1].Points.Where(p => p.id != -1).ToArray();
                for (int i = 0; i < validPoints.Length; ++i)
                {
                    if (validPoints[i].isRemoved)
                    {
                        m_fastPointCloud.SetPointColor(validPoints[i].id, new Color(0, 0, 0, 0));
                    }
                    else
                    {
                        m_fastPointCloud.SetPointColor(validPoints[i].id, Color.black);
                    }
                }
            }

            if (m_events[m_position].Points != null)
            {
                SlamPoint[] validPoints = m_events[m_position].Points.Where(p => p.id != -1).ToArray();
                for (int i = 0; i < validPoints.Length; ++i)
                {
                    Vector3 curPointPosition;
                    Color curPointColor;
                    m_fastPointCloud.GetPoint(validPoints[i].id, out curPointPosition, out curPointColor);
                    m_fastPointCloud.SetPoint(validPoints[i].id, curPointPosition + validPoints[i].position, validPoints[i].color);
                }
                if (needRepaint)
                    m_fastPointCloud.Repaint();
            }

            

            if (m_events[m_position].Observations != null)
            {
                SlamObservation[] observations = m_events[m_position].Observations;
                for (int i = 0; i < observations.Length; ++i)
                {
                    SlamObservation observationUpdate = observations[i];
                    if (!m_observationsGraph.ObservationExists(observationUpdate.id)) // новый observation
                    {
                        m_observationsGraph.AddNewObservation(observationUpdate);
                    }
                    else if (observationUpdate.isRemoved == true) // удалённый observation
                    {
                        m_observationsGraph.RemoveObservation(observationUpdate.id);
                    }
                    else if (observationUpdate.id == -1) // камера
                    {
                        m_observationsGraph.ReplaceObservation(observationUpdate);
                    }
                    else // перемещение ключевого observation
                    {
                        SlamObservation currentObservation = m_observationsGraph.GetObservationNode(observationUpdate.id);
                        Matrix4x4 currentOrientation = Matrix4x4.TRS(currentObservation.position, currentObservation.orientation.normalized, Vector3.one);
                        Matrix4x4 relativeOrientation = Matrix4x4.TRS(observationUpdate.position, observationUpdate.orientation.normalized, Vector3.one);
                        Matrix4x4 newOrientation = currentOrientation * relativeOrientation.inverse;
                        observationUpdate.position = newOrientation.GetColumn(3);
                        observationUpdate.orientation = Quaternion.LookRotation(newOrientation.GetColumn(2), newOrientation.GetColumn(1));
                        m_observationsGraph.ReplaceObservation(observationUpdate);
                    }
                }
                if (needRepaint)
                    m_observationsGraph.Repaint();
            }

            if (m_events[m_position].IsKeyEvent)
            {
                loggerListView.PushItem(m_events[m_position].ToString());
            }

            Debug.LogFormat("{0}. {1}", m_position, m_events[m_position]);
            return true;
        }

        bool PrevEvent(bool needRepaint)
        {
            if (m_position <= -1)
                return false;
            if (m_position == m_events.Length)
            {
                m_position -= 1;
                return true;
            }
            Debug.AssertFormat(m_position >= 0, "Position is {0}, but count of events is {1}", m_position, m_events.Length);
            if (m_backwardEventsPoints[m_position] != null)
            {
                SlamPoint[] points = m_backwardEventsPoints[m_position];
                for (int pointId = 0; pointId < points.Length; ++pointId)
                {
                    m_fastPointCloud.SetPoint(points[pointId].id, points[pointId].position, points[pointId].color);
                }
                
                if (needRepaint)
                    m_fastPointCloud.Repaint();
            }
            if (m_backwardEventsObservations[m_position] != null)
            {
                SlamObservation[] observations = m_backwardEventsObservations[m_position];
                for (int i = 0; i < observations.Length; ++i)
                {
                    SlamObservation observation = observations[i];
                    if (observation.isRemoved)
                    {
                        m_observationsGraph.RemoveObservation(observation.id);
                    }
                    else if (!m_observationsGraph.ObservationExists(observation.id))
                    {
                        m_observationsGraph.AddNewObservation(observation);
                    }
                    else
                    {
                        m_observationsGraph.ReplaceObservation(observation);
                    }
                }
            }

            if (m_events[m_position].IsKeyEvent)
            {
                loggerListView.PopItem();
            }

            m_position -= 1;
            Debug.Log(m_position);
            Debug.LogFormat("{0}. {1}", m_position, m_events[m_position]);

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
                    SlamPoint[] nextEventPoints = nextEvent.Points.Where(p => p.id != -1).ToArray();
                    SlamPoint[] currentPointsFromNextEvent = new SlamPoint[nextEventPoints.Length];
                    for (int pointIdx = 0; pointIdx < currentPointsFromNextEvent.Length; ++pointIdx)
                    {
                        currentPointsFromNextEvent[pointIdx].id = nextEventPoints[pointIdx].id;
                        m_fastPointCloud.GetPoint(currentPointsFromNextEvent[pointIdx].id, out currentPointsFromNextEvent[pointIdx].position, out currentPointsFromNextEvent[pointIdx].color);
                    }
                    m_backwardEventsPoints[eventIdx] = currentPointsFromNextEvent;
                }

                if (nextEvent.Observations != null)
                {
                    SlamObservation[] nextEventObservations = nextEvent.Observations;
                    SlamObservation[] currentObservations = new SlamObservation[nextEventObservations.Length];
                    for (int i = 0; i < nextEventObservations.Length; ++i)
                    {
                        if (m_observationsGraph.ObservationExists(nextEventObservations[i].id))
                        {
                            currentObservations[i] = m_observationsGraph.GetObservationNode(nextEventObservations[i].id);
                        }
                        else
                        {
                            currentObservations[i] = nextEventObservations[i];
                            currentObservations[i].isRemoved = !nextEventObservations[i].isRemoved;
                        }
                    }
                    m_backwardEventsObservations[eventIdx] = currentObservations;
                }
                NextEvent(needRepaint: false);
            }
            Stop();
            m_readyToWork = true;
            yield return null;
        }

    }
}