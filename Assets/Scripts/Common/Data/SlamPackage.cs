using Elektronik.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class SlamPackage : IPackage
    {
        private enum ObjectType : byte
        {
            SlamPoint = 0,
            SlamObservation = 1,
        }
        public PackageType Type { get => PackageType.SLAMPackage; }
        public int Timestamp { get; private set; }
        public bool IsKey { get; private set; }
        public List<SlamObservation> Observations { get; private set; }
        public List<SlamPoint> Points { get; private set; }
        public List<SlamLine> Lines { get; private set; }
        public string EventType { get; private set; }
        private string m_summary;
        private void EvaluateSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Package]")
              .AppendLine($"{EventType}")
              .AppendLine($"Timestamp: {Timestamp}")
              .AppendLine($"Is key: {IsKey}")
              .AppendLine($"New points count: {Points.Count(p => p.isNew)}")
              .AppendLine($"Tinted points count: {Points.Count(p => p.justColored)}")
              .AppendLine($"Removed points count: {Points.Count(p => p.isRemoved)}")
              .AppendLine($"Total count of points: {Points.Count}")
              .AppendLine($"New observations count: {Observations.Count(o => o.Point.isNew)}")
              .AppendLine($"Removed observations count: {Observations.Count(o => o.Point.isRemoved)}")
              .AppendLine($"Total count of observations: {Observations.Count}");
            m_summary = sb.ToString();
        }
        public SlamPackage()
        {
            Observations = new List<SlamObservation>(20);
            Points = new List<SlamPoint>(1000);
            Lines = new List<SlamLine>(500);
        }
        public static int Parse(byte[] rawPackage, int startIdx, out SlamPackage result)
        {
            result = null;
            int offset = startIdx;
            if ((PackageType)rawPackage[offset++] != PackageType.SLAMPackage)
                return 0;
            result = new SlamPackage();
            result.Timestamp = BitConverterEx.ToInt32(rawPackage, offset, ref offset);
            result.IsKey = BitConverterEx.ToBoolean(rawPackage, offset, ref offset);
            int sizeInBytesOfEventType = BitConverterEx.ToInt32(rawPackage, offset, ref offset);
            result.EventType = sizeInBytesOfEventType > 0 ?
                Encoding.ASCII.GetString(rawPackage, offset, sizeInBytesOfEventType) :
                "";
            offset += sizeInBytesOfEventType;
            int countOfObjects = BitConverterEx.ToInt32(rawPackage, offset, ref offset);

            for (int i = 0; i < countOfObjects; ++i)
            {
                int objectId = BitConverterEx.ToInt32(rawPackage, offset, ref offset);
                ObjectType objectType = (ObjectType)rawPackage[offset++];
                int actionsCount = rawPackage[offset++];
                int actionsSize = 0;
                for (int actionIdx = 0; actionIdx < actionsCount; ++actionIdx)
                {
                    actionsSize +=
                        objectType == ObjectType.SlamPoint
                        ? SlamPointsPackageObject.GetSizeOfActionInBytes((ActionType)rawPackage[offset + actionsSize])
                        : SlamObservationPackageObject.GetSizeOfActionInBytes((ActionType)rawPackage[offset + actionsSize]);
                    ++actionsSize; // type byte
                }
                byte[] actions = new byte[actionsSize];
                Debug.Assert(
                    offset + actionsSize <= rawPackage.Length,
                    $"[Package.Parse] Wrong size of action. actionSize + offset = {actionsSize + offset}, but size of package is {rawPackage.Length}");
                Buffer.BlockCopy(rawPackage, offset, actions, 0, actionsSize);
                offset += actionsSize;
                SlamLine? line = null;
                if (objectType == ObjectType.SlamPoint)
                {
                    SlamPointsPackageObject.ParseActions(actions, objectId, out SlamPoint point, out line);
                    result.Points.Add(point);
                }
                else
                {
                    SlamObservationPackageObject.ParseActions(actions, objectId, out SlamObservation observation);
                    result.Observations.Add(observation);
                }
                if (line != null)
                    result.Lines.Add(line.Value);
            }
            if (result.Observations.Count > 0)
            {
                if (result.Observations[0].Point.id != -1)
                    result.IsKey = true;
            }
            result.EvaluateSummary();
            return offset - startIdx;
        }


        private void Clear()
        {
            Observations.Clear();
            Lines.Clear();
            Points.Clear();
        }

        public override string ToString()
        {
            return m_summary;
        }
    }
}
