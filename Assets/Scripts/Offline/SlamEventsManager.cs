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
        List<ISlamEvent> m_extendedEvents;

        public SlamObservationsGraph observationsGraph;
        public FastPointCloud fastPointCloud;
        public FastLinesCloud fastLineCloud;
        public Helmet helmet;

        private SlamLinesContainer m_linesContainer;
        private SlamPointsContainer m_pointsContainer;

        private int m_position = -1;

        void Awake()
        {
            m_extendedEvents = new List<ISlamEvent>();
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
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            helmet.ResetHelmet();
        }

        public void Repaint()
        {
            observationsGraph.Repaint();
            m_pointsContainer.Repaint();
            m_linesContainer.Repaint();
        }

        public int GetLength()
        {
            return m_commands.Count;
        }

        public int GetCurrentEventPosition()
        {
            return m_position;
        }

        public ISlamEvent GetCurrentEvent()
        {
            if (m_position == -1) // до свершения какого либо события
                return null;
            return m_extendedEvents[m_position];
        }

        public void SetPosition(int pos)
        {
            if (!ReadyToPlay)
                return;
            int maxLength = GetLength();
            Debug.AssertFormat(pos >= 0 && pos < maxLength, "[SlamEventsManger.SetPosition] out of range pos == {0}, but range is [0,{1})", pos, maxLength);
            StartCoroutine(MoveToPostion(pos));
        }

        IEnumerator MoveToPostion(int pos)
        {
            ReadyToPlay = false;
            while (m_position != pos)
            {
                if (pos > m_position)
                    Next(false);
                if (pos < m_position)
                    Previous(false);
                if (m_position % 10 == 0)
                    yield return null;
            }
            Repaint();
            ReadyToPlay = true;
            yield return null;
        }

        public bool Next(bool needRepaint = true)
        {
            if (m_position < m_commands.Count - 1)
            {
                m_commands[++m_position].Execute();
                if (needRepaint)
                {
                    Repaint();
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
                    Repaint();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private int FindNextKeyEventIdx(int srcIdx)
        {
            for (int i = srcIdx; i < m_extendedEvents.Count; ++i)
            {
                if (m_extendedEvents[i].IsKeyEvent && (m_commands[i] is UpdateCommand))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool NextKeyEvent()
        {
            int idxOfKeyEvent = FindNextKeyEventIdx(m_position + 1);
            if (idxOfKeyEvent == -1)
                return false;
            while (m_position != idxOfKeyEvent)
            {
                Next(needRepaint: false);
            }
            Repaint();
            return true;
        }

        private int FindPrevKeyEventIdx(int srcIdx)
        {
            for (int i = srcIdx; i >= 0; --i)
            {
                if (m_extendedEvents[i].IsKeyEvent && (m_commands[i] is UpdateCommand))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool PrevKeyEvent()
        {
            if (m_position == GetLength())
                Previous(needRepaint: false);
            int idxOfKeyEvent = FindPrevKeyEventIdx(m_position - 1);
            if (idxOfKeyEvent == -1)
                return false;
            while (m_position != idxOfKeyEvent)
            {
                Previous(needRepaint: false);
            }
            Repaint();
            return true;
        }

        IEnumerator ProcessEvents()
        {
            Debug.Log("PROCESSING STARTED");

            for (int i = 0; i < m_events.Length; ++i)
            {
                //Debug.Log(m_events[i].EventType.ToString());
                if (m_events[i] is GlobalMapEvent)
                {
                    m_commands.Add(new ClearCommand(m_pointsContainer, m_linesContainer, observationsGraph));
                    m_extendedEvents.Add(m_events[i]);
                    Next(false);
                }
                m_commands.Add(new AddCommand(m_pointsContainer, m_linesContainer, observationsGraph, m_events[i]));
                m_extendedEvents.Add(m_events[i]);
                Next(false);
                m_commands.Add(new UpdateCommand(m_pointsContainer, observationsGraph, helmet, m_events[i]));
                m_extendedEvents.Add(m_events[i]);
                Next(false);
                m_commands.Add(new PostProcessingCommand(m_pointsContainer, m_linesContainer, observationsGraph, helmet, m_events[i]));
                m_extendedEvents.Add(m_events[i]);
                Next(false);

                if (i % 10 == 0)
                {
                    Debug.LogFormat("{0}. event {1}", i, m_events[i].ToString());
                    yield return null;
                }
            }
            Clear();
            Repaint();
            ReadyToPlay = true;
            Debug.Log("PROCESSING FINISHED");
            yield return null;
        }
    }
}
