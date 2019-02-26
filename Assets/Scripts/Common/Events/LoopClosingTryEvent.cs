using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Common.Events
{
    public class LoopClosingTryEvent : ISlamEvent
    {
        public SlamEventType EventType { get; set; }
        public int Timestamp { get; set; }
        public bool IsKeyEvent { get; set; }

        public int CandidatesCount { get; set; }

        public SlamObservation[] Observations { get; set; }
        public SlamPoint[] Points { get; private set; }
        public SlamLine[] Lines { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("LOOP CLOSING")
              .AppendFormat("Timestamp: {0}", TimeSpan.FromMilliseconds(Timestamp).ToString())
              .AppendLine()
              .AppendFormat("Candidates count: {0}", CandidatesCount)
              .AppendLine();
            foreach (var observation in Observations)
            {
                sb.AppendFormat("({0}; {1}; {2}; {3})", 
                    observation.statistics1, 
                    observation.statistics2, 
                    observation.statistics3,
                    observation.statistics4)
                  .AppendLine();

            }
            return sb.ToString();
        }

        public LoopClosingTryEvent()
        {
            EventType = SlamEventType.LCLoopClosingTry;
            IsKeyEvent = true;
        }

        public static LoopClosingTryEvent Parse(BinaryReader stream)
        {
            LoopClosingTryEvent parsed = new LoopClosingTryEvent();
            parsed.Timestamp = stream.ReadInt32();
            parsed.CandidatesCount = stream.ReadInt32();
            parsed.Points = null;
            parsed.Observations = new SlamObservation[parsed.CandidatesCount];
            for (int i = 0; i < parsed.CandidatesCount; ++i)
            {
                parsed.Observations[i] = new SlamObservation();
                parsed.Observations[i].id = stream.ReadInt32();
            }

            for (int i = 0; i < parsed.CandidatesCount; ++i)
            {
                parsed.Observations[i].statistics1 = stream.ReadByte();
                parsed.Observations[i].statistics2 = stream.ReadByte();
                parsed.Observations[i].statistics3 = stream.ReadByte();
                parsed.Observations[i].statistics4 = stream.ReadByte();
            }

            return parsed;
        }
    }
}
