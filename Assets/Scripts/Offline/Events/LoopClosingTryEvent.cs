using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.Offline.Events
{
    public class LoopClosingTryEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        public int Timestamp { get; private set; }
        public bool IsKeyEvent { get; private set; }

        public byte CandidatesCount { get; private set; }

        public SlamObservation[] Observations { get; private set; }

        public SlamPoint[] Points { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("LOOP CLOSING")
              .AppendFormat("Timestamp: {0}", TimeSpan.FromMilliseconds(Timestamp).ToString())
              .AppendLine()
              .AppendFormat("Candidates count: {0}", CandidatesCount);
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
            parsed.CandidatesCount = stream.ReadByte();
            parsed.Points = null;
            parsed.Observations = new SlamObservation[parsed.CandidatesCount];
            for (int i = 0; i < parsed.CandidatesCount; ++i)
            {
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
