using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Events
{
    public class MapModificationEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        public int Timestamp { get; private set; }
        public bool IsKeyEvent { get; private set; }

        public int MovedPtsCount { get; private set; }
        public int MovedObservationsCount { get; private set; }

        public SlamPoint[] Points { get; private set; }
        public SlamObservation[] Observations { get; private set; }
        public SlamLine[] Lines { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MAP MODIFICATION")
              .AppendFormat("Timestamp: {0}", TimeSpan.FromMilliseconds(Timestamp).ToString())
              .AppendLine()
              .AppendFormat("Modification type: {0}", EventType.ToString())
              .AppendFormat("Moved points count: {0}", MovedPtsCount)
              .AppendLine()
              .AppendFormat("Moved observations count: {0}", MovedObservationsCount);
            return sb.ToString();
        }

        public MapModificationEvent(SlamEventType type)
        {
            Debug.Assert(type == SlamEventType.LMLBA || type == SlamEventType.LCGBA || type == SlamEventType.LCOptimizeEssentialGraph);
            EventType = type;
            IsKeyEvent = true;
        }

        public static ISlamEvent Parse(BinaryReader stream, SlamEventType type)
        {
            MapModificationEvent parsed = new MapModificationEvent(type);
            parsed.Timestamp = stream.ReadInt32();
            parsed.MovedPtsCount = stream.ReadInt32();

            parsed.Points = new SlamPoint[parsed.MovedPtsCount];
            
            for (int i = 0; i < parsed.MovedPtsCount; ++i)
            {
                parsed.Points[i].id = stream.ReadInt32();

                if (type == SlamEventType.LMLBA)
                {
                    parsed.Points[i].color = new Color32(0x19, 0x7b, 0xfc, 0xff);
                }
                else if (type == SlamEventType.LCGBA)
                {
                    parsed.Points[i].color = new Color32(0x59, 0x08, 0x08, 0xff);
                }
                else
                {
                    parsed.Points[i].color = new Color32(0x19, 0x7b, 0xfc, 0xff);
                }
            }

            for (int i = 0; i < parsed.MovedPtsCount; ++i)
            {
                parsed.Points[i].position.x = stream.ReadSingle();
                parsed.Points[i].position.y = stream.ReadSingle();
                parsed.Points[i].position.z = stream.ReadSingle();
            }

            //parsed.Points = parsed.Points.Where(p => p.position != Vector3.zero).ToArray();
            parsed.MovedPtsCount = parsed.Points.Length;

            parsed.MovedObservationsCount = stream.ReadInt32();

            parsed.Observations = new SlamObservation[parsed.MovedObservationsCount];
            for (int i = 0; i < parsed.MovedObservationsCount; ++i)
            {
                parsed.Observations[i] = new SlamObservation();
                parsed.Observations[i].id = stream.ReadInt32();
            }

            for (int i = 0; i < parsed.MovedObservationsCount; ++i)
            {
                Vector3 position = new Vector3()
                {
                    x = stream.ReadSingle(),
                    y = stream.ReadSingle(),
                    z = stream.ReadSingle(),
                };
                Quaternion rotation = new Quaternion()
                {
                    w = stream.ReadSingle(),
                    x = stream.ReadSingle(),
                    y = stream.ReadSingle(),
                    z = stream.ReadSingle()
                };
                parsed.Observations[i].position = position;
                parsed.Observations[i].orientation = rotation;
            }

            /*for (int i = 0; i < parsed.MovedObservationsCount; ++i)
            {
                int countOfNieghbors = stream.ReadInt32();
                parsed.Observations[i].covisibleObservationsIds = new List<int>(countOfNieghbors);
                parsed.Observations[i].covisibleObservationsOfCommonPointsCount = new List<int>(countOfNieghbors);
                for (int j = 0; j < countOfNieghbors; ++j)
                {
                    parsed.Observations[i].covisibleObservationsIds.Add(stream.ReadInt32());
                }
            }*/

            return parsed;
        }
    }
}
