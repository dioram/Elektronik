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
        private GState m_state;
        private ISlamEvent[] m_events;

        public GState NextEvent()
        {
            if (!EndOfFile)
            {
                m_state.Update(m_events[++Position]);
            }
            return m_state;
        }

        public GState PrevEvent()
        {
            if (!StartOfFile)
            {
                m_state.Update(m_events[--Position]);
            }
            return m_state;
        }

        public GState NextKeyEvent()
        {
            int idxOfNextKeyEvent = -1;
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
                    NextEvent();
                }
            }
            return m_state;
        }

        public GState PrevKeyEvent()
        {
            int idxOfNextKeyEvent = -1;
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
                    PrevEvent();
                }
            }
            return m_state;
        }

        public GState SetPosition(uint position)
        {
            Debug.Assert(position < Length && position >= 0);
            uint countOfIterations = (uint)Math.Abs(position - Position);
            if (position < Position)
            {
                for (int i = 0; i < countOfIterations; ++i)
                {
                    PrevEvent();
                }
            }
            else if (position > Position)
            {
                for (int i = 0; i < countOfIterations; ++i)
                {
                    NextEvent();
                }
            }
            return m_state;
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
                EndOfFile = m_position == Length;
                StartOfFile = m_position == 0;
            }
        }

        public EventFilePlayer(ISlamEvent[] events)
        {
            m_events = events;
            Length = m_events.Length;
            LengthInTime = TimeSpan.FromMilliseconds(m_events.Last().Timestamp) - TimeSpan.FromMilliseconds(m_events.First().Timestamp);
        }

        public TimeSpan CurrentTimestamp { get { return TimeSpan.FromMilliseconds(m_state.Timestamp); } }

        public int Length { get; private set; }

        public TimeSpan LengthInTime { get; private set; }
    }
}
