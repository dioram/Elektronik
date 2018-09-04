using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Offline.Events;

namespace Elektronik.Offline
{
    public class GState : ICloneable
    {
        public Pose Pose { get; private set; }
        public uint Timestamp { get; private set; }
        public SortedDictionary<int, SlamPoint> PointCloud { get; private set; }
        public ISlamEvent LastEvent { get; private set; }

        public GState CloneUpdate(ISlamEvent slamEvent)
        {
            GState clone = Clone() as GState;
            clone.Update(slamEvent);
            return clone;
        }

        public void Update(ISlamEvent slamEvent, bool isInversed = false)
        {
            LastEvent = slamEvent;
            foreach (var point in PointCloud)
            {
                point.Value.Color = Color.black;
            }
            switch (slamEvent.EventType)
            {
                case SlamEventType.MainThreadEvent:
                    {
                        Update(slamEvent as MainThreadEvent);
                        break;
                    }
                case SlamEventType.LMPointsRemoval:
                    {
                        Update(slamEvent as LMPointsRemovalEvent, isInversed);
                        break;
                    }
                case SlamEventType.LMPointsFusion:
                    {
                        Update(slamEvent as LMPointsFusionEvent);
                        break;
                    }
                case SlamEventType.LMObservationRemoval:
                    {
                        Update(slamEvent as LMObservationRemovalEvent);
                        break;
                    }
                case SlamEventType.LMLBA:
                    {
                        Update(slamEvent as LMLBAEvent);
                        break;
                    }
                default:
                    break;
            }
        }

        public void Update(MainThreadEvent mainThreadEvent)
        {
            for (int i = 0; i < mainThreadEvent.NewPointsCount; ++i)
            {
                PointCloud.Add(mainThreadEvent.NewPointsIds[i], new SlamPoint(mainThreadEvent.NewPointsCoordinates[i], Color.gray));
            }
            for (int i = 0; i < mainThreadEvent.RecalcPointsCount; ++i)
            {
                PointCloud[mainThreadEvent.RecalcPointsIds[i]].Color = Color.black;
            }
            for (int i = 0; i < mainThreadEvent.RecognizedPointsCount; ++i)
            {
                PointCloud[mainThreadEvent.RecognizedPointsIds[i]].Color = Color.green;
            }
        }

        public void Update(LMPointsRemovalEvent lMPointsRemovalEvent, bool isInversed = false)
        {
            for (int i = 0; i < lMPointsRemovalEvent.RemovedMapPointsNumber; ++i)
            {
                if (isInversed)
                {
                    PointCloud[lMPointsRemovalEvent.RemovedMapPointsIds[i]].IsRemoved = true;
                }
                else
                {
                    PointCloud[lMPointsRemovalEvent.RemovedMapPointsIds[i]].IsRemoved = false;
                }
            }
        }

        public void Update(LMPointsFusionEvent lMPointsFusionEvent)
        {
            for (int i = 0; i < lMPointsFusionEvent.FusedMapPointsCount; ++i)
            {
                PointCloud[lMPointsFusionEvent.ReplacedMapPointsIds[i]].Color = Color.blue;
                PointCloud[lMPointsFusionEvent.ReplacingMapPointsIds[i]].Color = Color.cyan;
            }
        }

        public void Update(LMObservationRemovalEvent lMObservationRemovalEvent)
        {
            // TODO: спросить что это за штука и как она должна отображаться в сцене
            throw new NotImplementedException();
        }

        public void Update(LMLBAEvent lMLBAEvent, bool isInverted)
        {
            for (int i = 0; i < lMLBAEvent.MovedPtsCount; ++i)
            {
                if (!isInverted)
                {
                    PointCloud[lMLBAEvent.MovedPtsIds[i]].Position += lMLBAEvent.RelativeOffsetsOfMovedPts[i];
                }
                else
                {
                    PointCloud[lMLBAEvent.MovedPtsIds[i]].Position -= lMLBAEvent.RelativeOffsetsOfMovedPts[i];
                }
            }
            for (int i = 0; i < lMLBAEvent.MovedObservationsCount; ++i)
            {
                // TODO: спросить что это за штука и как она должна отображаться в сцене
                throw new NotImplementedException();
            }
        }

        public object Clone()
        {
            GState clone = new GState();
            clone.Pose = Pose;
            clone.Timestamp = Timestamp;
            clone.PointCloud = new SortedDictionary<int, SlamPoint>(PointCloud);
            return clone;
        }
    }
}
