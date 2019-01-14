using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class Package
    {
        public int Timestamp { get; private set; }
        public List<SlamObservation> Observations { get; private set; }
        public List<SlamPoint> Points { get; private set; }
        public List<SlamLine> Lines { get; private set; }

        public Package()
        {
            Observations = new List<SlamObservation>();
            Points = new List<SlamPoint>();
            Lines = new List<SlamLine>();
        }

        public static Package Parse(byte[] rawPackage)
        {
            Package result = new Package();
            int offset = 0;
            result.Timestamp = BitConverter.ToInt32(rawPackage, 0);
            offset += sizeof(int);
            int countOfObjects = BitConverter.ToInt32(rawPackage, offset);
            offset += sizeof(int);

            for (int i = 0; i < countOfObjects; ++i)
            {
                int objectId = BitConverter.ToInt32(rawPackage, offset);
                offset += sizeof(int);

                byte objectType = rawPackage[offset++];

                int actionsCount = BitConverter.ToInt32(rawPackage, offset);
                offset += sizeof(int);

                int actionsSize = 0;
                for (int actionIdx = 0; actionIdx < actionsCount; ++actionIdx)
                {
                    actionsSize +=
                        objectType == 0
                        ? SlamPointsPackageObject.GetSizeOfActionInBytes((ActionType)rawPackage[offset + actionsSize])
                        : SlamObservationPackageObject.GetSizeOfActionInBytes((ActionType)rawPackage[offset + actionsSize]);
                    ++actionsSize; // type byte
                }
                byte[] actions = new byte[actionsSize];
                Array.Copy(rawPackage, offset, actions, 0, actionsSize);
                offset += actionsSize;
                SlamLine? line = null;
                if (objectType == 0)
                {
                    SlamPoint point = new SlamPoint();
                    SlamPointsPackageObject.ParseActions(actions, objectId, out point, out line);
                    result.Points.Add(point);
                }
                else
                {
                    SlamObservation observation = null;
                    SlamObservationPackageObject.ParseActions(actions, objectId, out observation);
                }
                if (line.HasValue)
                    result.Lines.Add(line.Value);
            }
            return result;
        }
    }
}
