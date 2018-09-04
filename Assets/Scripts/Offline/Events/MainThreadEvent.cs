using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Elektronik.Offline.Events
{
    public class MainThreadEvent : ISlamEvent
    {
        public SlamEventType EventType { get; private set; }
        
        public uint Timestamp { get; private set; }
        public uint NewPointsCount { get; private set; }
        public int[] NewPointsIds { get; private set; }
        public Vector3[] NewPointsCoordinates { get; private set; }
        public int RecognizedPointsCount { get; private set; }
        public int[] RecognizedPointsIds { get; private set; }
        public int RecalcPointsCount { get; private set; }
        public int[] RecalcPointsIds { get; private set; }
        public int LocalPointsCount { get; private set; }
        public int[] LocalPointsIds { get; private set; }
        public int NewKeyObservationId { get; private set; }
        public Pose FramePose { get; private set; }
        public byte CovisibleObservationsCount { get; private set; }
        public int[] CovisibleObservationsIds { get; private set; }
        public int[] CovisibleObservationsOfCommonPointsCount { get; private set; }

        public MainThreadEvent()
        {
            EventType = SlamEventType.MainThreadEvent;
        }

        public static MainThreadEvent Parse(BinaryReader stream)
        {
            MainThreadEvent parsed = new MainThreadEvent();

            parsed.Timestamp = stream.ReadUInt32();

            parsed.NewPointsCount = stream.ReadUInt32();
            parsed.NewPointsIds = new int[parsed.NewPointsCount];
            parsed.NewPointsCoordinates = new Vector3[parsed.NewPointsCount];
            for (int i = 0; i < parsed.NewPointsCount; ++i)
            {
                parsed.NewPointsIds[i] = stream.ReadInt32();
            }
            for (int i = 0; i < parsed.NewPointsCount; ++i)
            {
                parsed.NewPointsCoordinates[i * 3].x = stream.ReadSingle();
                parsed.NewPointsCoordinates[i * 3 + 1].y = stream.ReadSingle();
                parsed.NewPointsCoordinates[i * 3 + 2].z = stream.ReadSingle();
            }
                
            parsed.RecognizedPointsCount = stream.ReadInt32();
            parsed.RecognizedPointsIds = new int[parsed.RecognizedPointsCount];
            for (int i = 0; i < parsed.RecognizedPointsCount; ++i)
            {
                parsed.RecognizedPointsIds[i] = stream.ReadInt32();
            }

            parsed.RecalcPointsCount = stream.ReadInt32();
            parsed.RecalcPointsIds = new int[parsed.RecalcPointsCount];
            for (int i = 0; i < parsed.RecalcPointsCount; ++i)
            {
                parsed.RecalcPointsIds[i] = stream.ReadInt32();
            }

            parsed.LocalPointsCount = stream.ReadInt32();
            parsed.LocalPointsIds = new int[parsed.LocalPointsCount];
            for (int i = 0; i < parsed.LocalPointsCount; ++i)
            {
                parsed.LocalPointsIds[i] = stream.ReadInt32();
            }

            parsed.NewKeyObservationId = stream.ReadInt32();

            float x = stream.ReadSingle(), y = stream.ReadSingle(), z = stream.ReadSingle();
            float qw = stream.ReadSingle(), qx = stream.ReadSingle(), qy = stream.ReadSingle(), qz = stream.ReadSingle();
            parsed.FramePose = new Pose(new Vector3(x, y, z), new Quaternion(qx, qy, qz, qw));

            parsed.CovisibleObservationsCount = stream.ReadByte();
            parsed.CovisibleObservationsIds = new int[parsed.CovisibleObservationsCount];
            parsed.CovisibleObservationsOfCommonPointsCount = new int[parsed.CovisibleObservationsCount];
            for (int i = 0; i < parsed.CovisibleObservationsCount; ++i)
            {
                parsed.CovisibleObservationsIds[i] = stream.ReadInt32();
            }
            for (int i = 0; i < parsed.CovisibleObservationsCount; ++i)
            {
                parsed.CovisibleObservationsOfCommonPointsCount[i] = stream.ReadInt32();
            }

            return parsed;
        }
    }
}
