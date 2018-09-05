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
        public SortedDictionary<int, SlamObservation> Observations { get; private set; }
        public ISlamEvent LastEvent { get; private set; }

        public GState()
        {
            PointCloud = new SortedDictionary<int, SlamPoint>();
            Observations = new SortedDictionary<int, SlamObservation>();
        }

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
                        Update(slamEvent as RemovalEvent, isInversed);
                        break;
                    }
                case SlamEventType.LMPointsFusion:
                    {
                        Update(slamEvent as PointsFusionEvent);
                        break;
                    }
                case SlamEventType.LMObservationRemoval:
                    {
                        Update(slamEvent as RemovalEvent, isInversed);
                        break;
                    }
                case SlamEventType.LMLBA:
                    {
                        Update(slamEvent as MapModificationEvent, isInversed);
                        break;
                    }
                case SlamEventType.LCLoopClosingTry:
                    {
                        Update(slamEvent as LoopClosingTryEvent);
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
                SlamPoint newPoint = new SlamPoint()
                {
                    Position = mainThreadEvent.NewPointsCoordinates[i],
                    Color = Color.gray,
                    IsRemoved = false,
                };
                PointCloud.Add(mainThreadEvent.NewPointsIds[i], newPoint);
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

        public void Update(RemovalEvent lMPointsRemovalEvent, bool isInversed = false)
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

        public void Update(PointsFusionEvent lMPointsFusionEvent)
        {
            // TODO: учесть тип события
            for (int i = 0; i < lMPointsFusionEvent.FusedMapPointsCount; ++i)
            {
                PointCloud[lMPointsFusionEvent.ReplacedMapPointsIds[i]].Color = Color.blue;
                PointCloud[lMPointsFusionEvent.ReplacingMapPointsIds[i]].Color = Color.cyan;
            }
        }

        public void Update(MapModificationEvent mapModificationEvent, bool isInverted)
        {
            // TODO: учесть тип события
            for (int i = 0; i < mapModificationEvent.MovedPtsCount; ++i)
            {
                if (!isInverted)
                {
                    PointCloud[mapModificationEvent.MovedPtsIds[i]].Position += mapModificationEvent.RelativeOffsetsOfMovedPts[i];
                }
                else
                {
                    PointCloud[mapModificationEvent.MovedPtsIds[i]].Position -= mapModificationEvent.RelativeOffsetsOfMovedPts[i];
                }
            }
            for (int i = 0; i < mapModificationEvent.MovedObservationsCount; ++i)
            {
                SlamObservation observation = Observations[mapModificationEvent.MovedObservationsIds[i]];
                Matrix4x4 currentObservationHomography = Matrix4x4.TRS(observation.Position, observation.Orientation, Vector3.one);
                Matrix4x4 relativeObservationHomography = Matrix4x4.TRS(
                    mapModificationEvent.RelativeOffsetsOfMovedObservations[i].position,
                    mapModificationEvent.RelativeOffsetsOfMovedObservations[i].rotation, 
                    Vector3.one);
                Matrix4x4 resultHomography;
                if (!isInverted)
                {
                    resultHomography = currentObservationHomography * relativeObservationHomography;
                }
                else
                {
                    resultHomography = currentObservationHomography * relativeObservationHomography.inverse;
                }
                observation.Position = resultHomography.GetColumn(3);
                observation.Orientation = Quaternion.LookRotation(resultHomography.GetColumn(2), resultHomography.GetColumn(1));
            }
        }

        public void Update(LoopClosingTryEvent loopClosingTryEvent)
        {
            for (int i = 0; i < loopClosingTryEvent.CandidatesCount; ++i)
            {
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics1 = loopClosingTryEvent.LoopClosingAttempts[i, 0];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics2 = loopClosingTryEvent.LoopClosingAttempts[i, 1];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics3 = loopClosingTryEvent.LoopClosingAttempts[i, 2];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics4 = loopClosingTryEvent.LoopClosingAttempts[i, 3];
            }
        }

        public object Clone()
        {
            GState clone = new GState
            {
                Pose = Pose,
                Timestamp = Timestamp,
                PointCloud = new SortedDictionary<int, SlamPoint>(PointCloud),
            };
            return clone;
        }
    }
}
