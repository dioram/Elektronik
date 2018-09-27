using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Events
{
    public class GlobalMapEvent : ISlamEvent
    {
        public SlamEventType EventType { get; set; }
        public int Timestamp { get; set; }
        public bool IsKeyEvent { get; set; }
        public SlamObservation[] Observations { get; set; }
        public SlamPoint[] Points { get; private set; }
        public SlamLine[] Lines { get; private set; }

        public int CountOfPoints { get; private set; }
        public int CountOfObservations { get; private set; }

        public GlobalMapEvent()
        {
            EventType = SlamEventType.GlobalMap;
        }

        public static GlobalMapEvent Parse(BinaryReader stream)
        {
            GlobalMapEvent parsed = new GlobalMapEvent();
            parsed.Timestamp = stream.ReadInt32();

            parsed.CountOfPoints = stream.ReadInt32();
            SlamPoint[] points = new SlamPoint[parsed.CountOfPoints];

            for (int i = 0; i < points.Length; ++i)
            {
                points[i].id = stream.ReadInt32();
                points[i].justColored = false;
                points[i].isRemoved = false;
                points[i].color = Color.black;
            }

            for (int i = 0; i < points.Length; ++i)
            {
                points[i].position.x = stream.ReadSingle();
                points[i].position.y = stream.ReadSingle();
                points[i].position.z = stream.ReadSingle();
            }
            parsed.Points = points;


            parsed.CountOfObservations = stream.ReadInt32();
            SlamObservation[] observations = new SlamObservation[parsed.CountOfObservations];

            for (int i = 0; i < observations.Length; ++i)
            {
                observations[i].id = stream.ReadInt32();
                observations[i].color = Color.gray;
                observations[i].isRemoved = false;
            }

            for (int i = 0; i < observations.Length; ++i)
            {
                observations[i].position.x = stream.ReadSingle();
                observations[i].position.y = stream.ReadSingle();
                observations[i].position.z = stream.ReadSingle();
                observations[i].orientation.w = stream.ReadSingle();
                observations[i].orientation.x = stream.ReadSingle();
                observations[i].orientation.y = stream.ReadSingle();
                observations[i].orientation.z = stream.ReadSingle();
            }


            for (int i = 0; i < observations.Length; ++i)
            {
                int countOfNieghbors = stream.ReadInt32();
                observations[i].covisibleObservationsIds = new int[countOfNieghbors];
                observations[i].covisibleObservationsOfCommonPointsCount = new int[countOfNieghbors];
                for (int j = 0; j < countOfNieghbors; ++j)
                {
                    observations[i].covisibleObservationsIds[j] = stream.ReadInt32();
                }
            }

            parsed.Observations = observations;

            return parsed;
        }
    }
}
