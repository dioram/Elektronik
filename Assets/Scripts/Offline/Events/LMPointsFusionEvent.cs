using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public class LMPointsFusionEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        public int Timestamp { get; private set; }
        public int FusedMapPointsCount { get; private set; }
        public int[] ReplacedMapPointsIds { get; private set; }
        public int[] ReplacingMapPointsIds { get; private set; }

        public LMPointsFusionEvent()
        {
            EventType = SlamEventType.LMPointsFusion;
        }

        public static LMPointsFusionEvent Parse(BinaryReader stream)
        {
            LMPointsFusionEvent parsed = new LMPointsFusionEvent();
            parsed.Timestamp = stream.ReadInt32();
            parsed.FusedMapPointsCount = stream.ReadInt32();
            for (int i = 0; i < parsed.FusedMapPointsCount; ++i)
            {
                parsed.ReplacedMapPointsIds[i] = stream.ReadInt32();
            }
            for (int i = 0; i < parsed.FusedMapPointsCount; ++i)
            {
                parsed.ReplacingMapPointsIds[i] = stream.ReadInt32();
            }
            return parsed;
        }
    }
}
