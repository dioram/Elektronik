using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public class RemovalEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }

        public int Timestamp { get; private set; }
        public int RemovedMapPointsNumber { get; private set; }
        public int[] RemovedMapPointsIds { get; private set; }

        public RemovalEvent(SlamEventType type)
        {
            Debug.Assert(type == SlamEventType.LMObservationRemoval || type == SlamEventType.LMPointsRemoval);
            EventType = SlamEventType.LMPointsRemoval;
        }

        public static RemovalEvent Parse(BinaryReader stream, SlamEventType type)
        {
            RemovalEvent parsed = new RemovalEvent(type);
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
