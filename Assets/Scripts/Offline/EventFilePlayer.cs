using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Offline
{
    class EventFilePlayer
    {
        GState[] m_timeline;
        uint m_currentFrame;

        public GState NextEvent()
        {
            return m_timeline[m_currentFrame++];
        }

        public GState NextKeyEvent()
        {
            GState nextEvent = null;
            for (uint i = m_currentFrame + 1; i < m_timeline.Length; ++i)
            {
                if (m_timeline[i].IsKeyEvent)
                {
                    nextEvent = m_timeline[i];
                    break;
                }
            }
            return nextEvent;
        }

        public GState PrevKeyEvent()
        {
            GState prevEvent = null;
            for (uint i = m_currentFrame - 1; i >= 0; --i)
            {
                if (m_timeline[i].IsKeyEvent)
                {
                    prevEvent = m_timeline[i];
                    break;
                }
            }
            return prevEvent;
        }

        public uint Position { get; set; }

        public uint Length { get; private set; }

        public TimeSpan LengthInTime { get; private set; }
    }
}
