using Elektronik.Common;
using Elektronik.Common.Events;
using Elektronik.Common.SlamEventsCommandPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamEventsManager : MonoBehaviour
    {
        public bool ReadyToPlay { get; private set; }

        public float scale;

        ISlamEvent[] m_events;
        List<ISlamEventCommand> m_commands;

        public SlamObservationsGraph observationsGraph;

        public FastPointCloud fastPointCloud;
        public FastLinesCloud fastLineCloud;
        private SlamLinesContainer m_linesContainer;
        private SlamPointsContainer m_pointsContainer;

        private int m_position = -1;

        void Awake()
        {
            m_commands = new List<ISlamEventCommand>();
        }

        void Start()
        {
            ISlamEventDataConverter converter = new Camera2Unity3dSlamEventConverter(Matrix4x4.Scale(Vector3.one * scale));
            m_events = EventReader.AnalyzeFile(FileModeSettings.Path, converter);
            m_linesContainer = new SlamLinesContainer(fastLineCloud);
            m_pointsContainer = new SlamPointsContainer(fastPointCloud);
            StartCoroutine(ProcessEvents());
        }

        public void Clear()
        {
            m_position = -1;
            observationsGraph.Clear();
            fastPointCloud.Clear();
            fastLineCloud.Clear();
        }

        public int GetLength()
        {
            return m_events.Length;
        }

        public int GetCurrentEventPosition()
        {
            return m_position / 2;
        }

        public ISlamEvent GetCurrentEvent()
        {
            if (m_position % 2 != 0)
            {
                return null;
            }
            else
            {
                return m_events[m_position / 2]; // делением на 2 пропускаем пост обработку
            }
        }

        public bool Next(bool needRepaint = true)
        {
            if (m_position < m_commands.Count - 1)
            {
                m_commands[++m_position].Execute();
                if (needRepaint)
                {
                    observationsGraph.Repaint();
                    fastPointCloud.Repaint();
                    fastLineCloud.Repaint();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Previous(bool needRepaint = true)
        {
            if (m_position != 0)
            {
                m_commands[m_position--].UnExecute();
                if (needRepaint)
                {
                    observationsGraph.Repaint();
                    fastPointCloud.Repaint();
                    fastLineCloud.Repaint();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NextKeyEvent()
        {
            int idxOfKeyEvent = Array.FindIndex(m_events, GetCurrentEventPosition() + 1, e => e.IsKeyEvent);
            if (idxOfKeyEvent == -1)
                return false;
            while (GetCurrentEventPosition() != idxOfKeyEvent)
            {
                Next(needRepaint: false);
            }
            fastLineCloud.Repaint();
            fastPointCloud.Repaint();
            observationsGraph.Repaint();
            return true;
        }

        public bool PrevKeyEvent()
        {
            int idxOfKeyEvent = -1;
            if (GetCurrentEventPosition() == GetLength())
                Previous(needRepaint: false);
            for (int i = GetCurrentEventPosition() - 1; i >= 0; --i)
            {
                if (m_events[i].IsKeyEvent)
                {
                    idxOfKeyEvent = i;
                    break;
                }
            }
            if (idxOfKeyEvent == -1)
                return false;
            while (GetCurrentEventPosition() != idxOfKeyEvent)
            {
                Previous(needRepaint: false);
            }
            Previous(needRepaint: false); // пропускаем пост обработку
            fastPointCloud.Repaint();
            observationsGraph.Repaint();
            return true;
        }

        IEnumerator ProcessEvents()
        {
            Debug.Log("Processing started");

            for (int i = 0; i < m_events.Length; ++i)
            {
                if (i % 10 == 0)
                {
                    Debug.LogFormat("{0}. event {1}", i, m_events[i].ToString());
                    yield return null;
                }
                Debug.Log(m_events[i].EventType.ToString());
                if (m_events[i] is GlobalMapEvent)
                {
                    m_commands.Add(new RepaintEntireMapCommand(m_pointsContainer, m_linesContainer, observationsGraph, m_events[i]));
                }
                else
                {
                    m_commands.Add(new SlamEventCommand(m_pointsContainer, m_linesContainer, observationsGraph, m_events[i]));
                    Next(false);
                    m_commands.Add(new PostProcessingCommand(m_pointsContainer, m_linesContainer, m_events[i]));
                    Next(false);
                }
            }
            m_position = -1;
            fastPointCloud.Clear();
            observationsGraph.Clear();
            ReadyToPlay = true;
            yield return null;
        }
    }
}
