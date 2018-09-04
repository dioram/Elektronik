using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public class LMPointsRemovalEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }

        public int Timestamp { get; private set; }
        public int RemovedMapPointsNumber { get; private set; }
        public int[] RemovedMapPointsIds { get; private set; }

        public LMPointsRemovalEvent()
        {
            EventType = SlamEventType.LMPointsRemoval;
        }

        public static LMPointsRemovalEvent Parse(BinaryReader stream)
        {
            LMPointsRemovalEvent parsed = new LMPointsRemovalEvent();

            parsed.Timestamp = stream.ReadInt32();

            parsed.RemovedMapPointsNumber = stream.ReadInt32();
            for (int i = 0; i < parsed.RemovedMapPointsNumber; ++i)
            {
                parsed.RemovedMapPointsIds[i] = stream.ReadInt32();
            }

            return parsed;
        }
    }
}
