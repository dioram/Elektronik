using Elektronik.Offline.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Elektronik.Offline
{
    public class EventFilePlayer
    {
        //private GState m_state;
        private ISlamEvent[] m_events;

        public ISlamEvent NextEvent()
        {
            UnityEngine.Debug.Log(Position);
            ISlamEvent result = m_events[Position];
            if (!EndOfFile)
            {
                result = m_events[++Position];
            }
            return result;
        }

        public ISlamEvent PrevEvent()
        {
            ISlamEvent result = m_events[Position];
            if (!StartOfFile)
            {
                result = m_events[--Position];
            }
            return result;
        }

        public ISlamEvent NextKeyEvent()
        {
            int idxOfNextKeyEvent = -1;
            ISlamEvent result = m_events[Position];
            for (int i = Position + 1; i < Length; ++i)
            {
                if (m_events[i].IsKeyEvent)
                {
                    idxOfNextKeyEvent = i;
                    break;
                }
            }
            if (idxOfNextKeyEvent != -1)
            {
                while (Position != idxOfNextKeyEvent)
                {
                    result = NextEvent();
                }
            }
            return result;
        }

        public ISlamEvent PrevKeyEvent()
        {
            int idxOfNextKeyEvent = -1;
            ISlamEvent result = m_events[Position];
            for (int i = Position - 1; i >= 0; --i)
            {
                if (m_events[i].IsKeyEvent)
                {
                    idxOfNextKeyEvent = i;
                    break;
                }
            }
            if (idxOfNextKeyEvent != -1)
            {
                while (Position != idxOfNextKeyEvent)
                {
                    result = PrevEvent();
                }
            }
            return result;
        }

        public ISlamEvent SetPosition(uint position)
        {
            Debug.Assert(position < Length && position >= 0);
            uint countOfIterations = (uint)Math.Abs(position - Position);
            ISlamEvent result = m_events[Position];
            if (position < Position)
            {
                for (int i = 0; i < countOfIterations; ++i)
                {
                    result = PrevEvent();
                }
            }
            else if (position > Position)
            {
                for (int i = 0; i < countOfIterations; ++i)
                {
                    result = NextEvent();
                }
            }
            return result;
        }

        public bool EndOfFile { get; private set; }
        public bool StartOfFile { get; private set; }

        private int m_position;
        public int Position
        {
            get {
                return m_position;
            }
            private set
            {
                m_position = value;
                EndOfFile = m_position == Length - 1;
                StartOfFile = m_position == 0;
            }
        }

        public EventFilePlayer(ISlamEvent[] events)
        {
            m_events = events;
            //m_state = new GState();
            Length = m_events.Length;
            LengthInTime = TimeSpan.FromMilliseconds(m_events.Last().Timestamp) - TimeSpan.FromMilliseconds(m_events.First().Timestamp);
            Position = 0;
        }

        public TimeSpan CurrentTimestamp { get { return TimeSpan.FromMilliseconds(m_events[Position].Timestamp); } }

        public int Length { get; private set; }

        public TimeSpan LengthInTime { get; private set; }
    }
}
