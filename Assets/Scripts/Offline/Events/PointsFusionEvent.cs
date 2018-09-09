using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public class PointsFusionEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        public int Timestamp { get; private set; }
        public bool IsKeyEvent { get; private set; }

        public int FusedMapPointsCount { get; private set; }
        public int[] ReplacedMapPointsIds { get; private set; }
        public int[] ReplacingMapPointsIds { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("POINTS FUSION")
              .AppendFormat("Timestamp: {0}", TimeSpan.FromMilliseconds(Timestamp).ToString())
              .AppendLine()
              .AppendFormat("Fused map points count: {0}", FusedMapPointsCount);
            return sb.ToString();
        }

        public PointsFusionEvent(SlamEventType type)
        {
            Debug.Assert(type == SlamEventType.LCPointsFusion || type == SlamEventType.LMPointsFusion);
            EventType = type;
            IsKeyEvent = true;
        }

        public static PointsFusionEvent Parse(BinaryReader stream, SlamEventType type)
        {
            PointsFusionEvent parsed = new PointsFusionEvent(type);
            parsed.Timestamp = stream.ReadInt32();
            parsed.FusedMapPointsCount = stream.ReadInt32();
            parsed.ReplacedMapPointsIds = new int[parsed.FusedMapPointsCount];
            parsed.ReplacingMapPointsIds = new int[parsed.FusedMapPointsCount];
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
