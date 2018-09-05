using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline.Events
{
    public class MapModificationEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }

        public int Timestamp { get; private set; }
        public int MovedPtsCount { get; private set; }
        public int[] MovedPtsIds { get; private set; }
        public Vector3[] RelativeOffsetsOfMovedPts { get; private set; }
        public int MovedObservationsCount { get; private set; }
        public int[] MovedObservationsIds { get; private set; }
        public Pose[] RelativeOffsetsOfMovedObservations { get; private set; }

        public MapModificationEvent(SlamEventType type)
        {
            Debug.Assert(type == SlamEventType.LMLBA || type == SlamEventType.LCGBA || type == SlamEventType.LCOptimizeEssentialGraph);
            EventType = type;
        }

        public static MapModificationEvent Parse(BinaryReader stream, SlamEventType type)
        {
            MapModificationEvent parsed = new MapModificationEvent(type);
            parsed.Timestamp = stream.ReadInt32();
            parsed.MovedPtsCount = stream.ReadInt32();
            parsed.MovedPtsIds = new int[parsed.MovedPtsCount];
            parsed.RelativeOffsetsOfMovedPts = new Vector3[parsed.MovedPtsCount];
            for (int i = 0; i < parsed.MovedPtsCount; ++i)
            {
                parsed.MovedPtsIds[i] = stream.ReadInt32();
            }
            for (int i = 0; i < parsed.MovedPtsCount; i += 3)
            {
                parsed.RelativeOffsetsOfMovedPts[i].x = stream.ReadSingle();
                parsed.RelativeOffsetsOfMovedPts[i + 1].y = stream.ReadSingle();
                parsed.RelativeOffsetsOfMovedPts[i + 2].z = stream.ReadSingle();
            }

            parsed.MovedObservationsCount = stream.ReadInt32();
            parsed.MovedObservationsIds = new int[parsed.MovedObservationsCount];
            parsed.RelativeOffsetsOfMovedObservations = new Pose[parsed.MovedObservationsCount];
            for (int i = 0; i < parsed.MovedObservationsCount; ++i)
            {
                parsed.MovedObservationsIds[i] = stream.ReadInt32();
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
                parsed.RelativeOffsetsOfMovedObservations[i] = new Pose(position, rotation);
            }
            return parsed;
        }
    }
}
