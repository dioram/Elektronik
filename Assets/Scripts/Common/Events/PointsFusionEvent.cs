using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Events
{
    public class PointsFusionEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        public int Timestamp { get; private set; }
        public bool IsKeyEvent { get; private set; }

        public SlamPoint[] Points { get; private set; }
        public SlamObservation[] Observations { get; private set; }
        public SlamLine[] Lines { get; private set; }


        public int FusedMapPointsCount { get; set; }

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
            UnityEngine.Debug.Assert(type == SlamEventType.LCPointsFusion || type == SlamEventType.LMPointsFusion);
            EventType = type;
            IsKeyEvent = true;
        }

        public static PointsFusionEvent Parse(BinaryReader stream, SlamEventType type)
        {
            PointsFusionEvent parsed = new PointsFusionEvent(type);
            parsed.Timestamp = stream.ReadInt32();
            parsed.FusedMapPointsCount = stream.ReadInt32();
            parsed.Observations = null;

            SlamPoint[] replaced = new SlamPoint[parsed.FusedMapPointsCount];
            SlamPoint[] replacing = new SlamPoint[parsed.FusedMapPointsCount];

            for (int i = 0; i < parsed.FusedMapPointsCount; ++i)
            {
                replaced[i].id = stream.ReadInt32();
                if (replaced[i].id != -1)
                {
                    replaced[i].color = new Color32(0xff, 0x00, 0xff, 0xff);
                    replaced[i].isRemoved = true;
                    replaced[i].justColored = true;
                }
            }
            for (int i = 0; i < parsed.FusedMapPointsCount; ++i)
            {
                replacing[i].id = stream.ReadInt32();
                if (replacing[i].id != -1)
                {
                    replacing[i].color = new Color32(0xff, 0x00, 0xff, 0xff);
                    replacing[i].justColored = true;
                }
            }

            List<SlamLine> lines = new List<SlamLine>(parsed.FusedMapPointsCount);
            for (int i = 0; i < parsed.FusedMapPointsCount; ++i)
            {
                if (replaced[i].id != -1 && replacing[i].id != -1)
                {
                    SlamLine line = new SlamLine()
                    {
                        color = replaced[i].color,
                        pointId1 = replaced[i].id,
                        pointId2 = replacing[i].id,
                    };
                    lines.Add(line);
                }
            }
            parsed.Lines = lines.ToArray();
            parsed.Points = replaced.Concat(replacing).ToArray();

            return parsed;
        }
    }
}
