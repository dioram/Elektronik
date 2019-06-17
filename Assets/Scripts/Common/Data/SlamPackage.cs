using System;
using System.Collections;
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
            TrajectoryPoint = 2,
        }
        public PackageType Type { get => PackageType.SLAMPackage; }
        public bool IsKey { get; private set; }
        public int Timestamp { get; private set; }
        public List<SlamObservation> Observations { get; private set; }
        public List<SlamPoint> Points { get; private set; }
        public List<SlamLine> Lines { get; private set; }
        public string EventType { get; private set; }
        private string m_summary;
        private void EvaluateSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Package]")
              .AppendLine()
              .AppendFormat("{0}", EventType)
              .AppendLine()
              .AppendFormat("Timestamp: {0}", Timestamp)
              .AppendLine()
              .AppendFormat("Is key: {0}", IsKey)
              .AppendLine()
              .AppendFormat("New points count: {0}", Points.Count(p => p.isNew))
              .AppendLine()
              .AppendFormat("Tinted points count: {0}", Points.Count(p => p.justColored))
              .AppendLine()
              .AppendFormat("Removed points count: {0}", Points.Count(p => p.isRemoved))
              .AppendLine()
              .AppendFormat("Total count of points: {0}", Points.Count)
              .AppendLine()
              .AppendFormat("New observations count: {0}", Observations.Count(o => o.Point.isNew))
              .AppendLine()
              .AppendFormat("Removed observations count: {0}", Observations.Count(o => o.Point.isRemoved))
              .AppendLine()
              .AppendFormat("Total count of observations: {0}", Observations.Count);
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
            var pkg = new SlamPackage();
            int offset = startIdx;
            pkg.Timestamp = BitConverter.ToInt32(rawPackage, 0);
            offset += sizeof(int);

            int sizeInBytesOfEventType = BitConverter.ToInt32(rawPackage, offset);
            offset += sizeof(int);

            pkg.EventType = sizeInBytesOfEventType > 0 ?
                Encoding.ASCII.GetString(rawPackage, offset, sizeInBytesOfEventType) :
                "";
            offset += sizeInBytesOfEventType;
            int countOfObjects = BitConverter.ToInt32(rawPackage, offset);
            offset += sizeof(int);

            for (int i = 0; i < countOfObjects; ++i)
            {
                int objectId = BitConverter.ToInt32(rawPackage, offset);
                offset += sizeof(int);
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
                Debug.AssertFormat(
                    offset + actionsSize <= rawPackage.Length,
                    "[Package.Parse] Wrong size of action. actionSize + offset = {0}, but size of package is {1}",
                    offset + actionsSize, rawPackage.Length);
                Buffer.BlockCopy(rawPackage, offset, actions, 0, actionsSize);
                offset += actionsSize;
                SlamLine? line = null;
                if (objectType == ObjectType.SlamPoint)
                {
                    SlamPointsPackageObject.ParseActions(actions, objectId, out SlamPoint point, out line);
                    pkg.Points.Add(point);
                }
                else
                {
                    SlamObservationPackageObject.ParseActions(actions, objectId, out SlamObservation observation);
                    pkg.Observations.Add(observation);
                }
                if (line != null)
                    pkg.Lines.Add(line.Value);
            }
            if (pkg.Observations.Count > 0)
            {
                if (pkg.Observations[0].Point.id != -1)
                    pkg.IsKey = true;
            }
            pkg.EvaluateSummary();
            result = pkg;
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
