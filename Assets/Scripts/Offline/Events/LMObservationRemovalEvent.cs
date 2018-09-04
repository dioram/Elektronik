using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public class LMObservationRemovalEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }

        public int Timestamp { get; private set; }
        public int RemovedObservationsCount { get; private set; }
        public int[] RemovedObservationsIds { get; private set; }

        public LMObservationRemovalEvent()
        {
            EventType = SlamEventType.LMObservationRemoval;
        }

        public static LMObservationRemovalEvent Parse(BinaryReader stream)
        {
            LMObservationRemovalEvent parsed = new LMObservationRemovalEvent();
            parsed.RemovedObservationsCount = stream.ReadInt32();
            for (int i = 0; i < parsed.RemovedObservationsCount; ++i)
            {
                parsed.RemovedObservationsIds[i] = stream.ReadInt32();
            }
            return parsed;
        }
    }
}
