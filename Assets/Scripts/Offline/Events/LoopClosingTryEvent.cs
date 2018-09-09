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
        public int[] CandidatesIds { get; private set; }
        public byte[,] LoopClosingAttempts { get; private set; }

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
            parsed.CandidatesIds = new int[parsed.CandidatesCount];
            for (int i = 0; i < parsed.CandidatesCount; ++i)
            {
                parsed.CandidatesIds[i] = stream.ReadInt32();
            }

            parsed.LoopClosingAttempts = new byte[parsed.CandidatesCount, 4];
            for (int i = 0; i < parsed.CandidatesCount; ++i)
            {
                parsed.LoopClosingAttempts[i, 0] = stream.ReadByte();
                parsed.LoopClosingAttempts[i, 1] = stream.ReadByte();
                parsed.LoopClosingAttempts[i, 2] = stream.ReadByte();
                parsed.LoopClosingAttempts[i, 3] = stream.ReadByte();
            }
            return parsed;
        }
    }
}
