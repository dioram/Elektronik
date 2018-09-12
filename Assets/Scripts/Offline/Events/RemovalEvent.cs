using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline.Events
{
    public class RemovalEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        public int Timestamp { get; private set; }
        public bool IsKeyEvent { get; private set; }

        public SlamPoint[] Points { get; private set; }
        public SlamObservation[] Observations { get; private set; }

        public int RemovedCount { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("REMOVAL")
              .AppendFormat("Timestamp: {0}", TimeSpan.FromMilliseconds(Timestamp).ToString())
              .AppendLine()
              .AppendFormat("Type of removal: {0}", EventType.ToString())
              .AppendLine()
              .AppendFormat("Removed count: {0}", RemovedCount);
            return sb.ToString();
        }

        public RemovalEvent(SlamEventType type)
        {
            UnityEngine.Debug.Assert(type == SlamEventType.LMObservationRemoval || type == SlamEventType.LMPointsRemoval);
            EventType = type;
        }

        public static RemovalEvent Parse(BinaryReader stream, SlamEventType type)
        {
            RemovalEvent parsed = new RemovalEvent(type);
            parsed.Timestamp = stream.ReadInt32();
            parsed.RemovedCount = stream.ReadInt32();

            if (type == SlamEventType.LMPointsRemoval)
            {
                parsed.Points = new SlamPoint[parsed.RemovedCount];
                parsed.Observations = null;
                for (int i = 0; i < parsed.RemovedCount; ++i)
                {
                    parsed.Points[i].id = stream.ReadInt32();
                    parsed.Points[i].color = Color.red;
                    parsed.Points[i].isRemoved = true;
                }
            }
            else
            {
                parsed.Observations = new SlamObservation[parsed.RemovedCount];
                parsed.Points = null;
                for (int i = 0; i < parsed.RemovedCount; ++i)
                {
                    parsed.Observations[i].id = stream.ReadInt32();
                    parsed.Observations[i].color = Color.red;
                    parsed.Observations[i].isRemoved = true;
                }
            }

            
            return parsed;
        }
    }
}
