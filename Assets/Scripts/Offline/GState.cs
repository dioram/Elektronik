using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Offline.Events;

namespace Elektronik.Offline
{
    public class GState
    {
        public Pose Pose { get; private set; }
        public uint Timestamp { get; private set; }
        public SortedDictionary<int, SlamPoint> PointCloud { get; private set; }
        public SortedDictionary<int, SlamObservation> Observations { get; private set; }
        public ISlamEvent CurrentEvent { get; private set; }
        public bool WasAnomaly { get; private set; }
        public string AnomalyDescription { get; private set; }


        private Stack<Dictionary<int, SlamPoint>> m_inversePointsHistory;
        private Stack<Dictionary<int, SlamObservation>> m_inverseObservationsHistory;

        public GState()
        {
            PointCloud = new SortedDictionary<int, SlamPoint>();
            Observations = new SortedDictionary<int, SlamObservation>();
            m_inversePointsHistory = new Stack<Dictionary<int, SlamPoint>>();
            m_inverseObservationsHistory = new Stack<Dictionary<int, SlamObservation>>();
        }

        public void Update(ISlamEvent slamEvent, bool isInversed = false)
        {
            CurrentEvent = slamEvent;
            Timestamp = (uint)slamEvent.Timestamp;

            if (isInversed)
            {
                var inversePoints = m_inversePointsHistory.Pop();
                var inverseObservations = m_inverseObservationsHistory.Pop();
                foreach (var point in inversePoints)
                {
                    PointCloud[point.Key] = point.Value;
                }
                foreach (var observation in inverseObservations)
                {
                    Observations[observation.Key] = observation.Value;
                }
                return;
            }

            m_inversePointsHistory.Push(new Dictionary<int, SlamPoint>());
            m_inverseObservationsHistory.Push(new Dictionary<int, SlamObservation>());

            switch (slamEvent.EventType)
            {
                case SlamEventType.MainThreadEvent:
                    {
                        Update(slamEvent as MainThreadEvent);
                        break;
                    }
                case SlamEventType.LMPointsRemoval:
                    {
                        Update(slamEvent as RemovalEvent);
                        break;
                    }
                case SlamEventType.LMPointsFusion:
                    {
                        Update(slamEvent as PointsFusionEvent);
                        break;
                    }
                case SlamEventType.LMObservationRemoval:
                    {
                        Update(slamEvent as RemovalEvent);
                        break;
                    }
                case SlamEventType.LMLBA:
                    {
                        Update(slamEvent as MapModificationEvent);
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
            var oldPointsStorage = m_inversePointsHistory.Peek();

            for (int i = 0; i < mainThreadEvent.NewPointsCount; ++i)
            {
                SlamPoint newPoint = new SlamPoint()
                {
                    Position = mainThreadEvent.NewPointsCoordinates[i],
                    Color = Color.red,
                    IsRemoved = false,
                };
                PointCloud.Add(mainThreadEvent.NewPointsIds[i], newPoint);
            }
            Debug.Log("New points added");
            for (int i = 0; i < mainThreadEvent.RecalcPointsCount; ++i)
            {
                oldPointsStorage.Add(mainThreadEvent.RecalcPointsIds[i], PointCloud[mainThreadEvent.RecalcPointsIds[i]].Clone());
                Debug.Log("Added in points history successful");
                PointCloud[mainThreadEvent.RecalcPointsIds[i]].Color = Color.black;
            }
            Debug.Log("Recalc points colored");
            for (int i = 0; i < mainThreadEvent.RecognizedPointsCount; ++i)
            {
                oldPointsStorage.Add(mainThreadEvent.RecognizedPointsIds[i], PointCloud[mainThreadEvent.RecognizedPointsIds[i]].Clone());
                Debug.Log("Added in points history successful");
                PointCloud[mainThreadEvent.RecognizedPointsIds[i]].Color = Color.green;
            }
            Debug.Log("Recognized points colored");
            if (mainThreadEvent.NewKeyObservationId > -1)
            {
                SlamObservation observation = new SlamObservation()
                {
                    Orientation = mainThreadEvent.ObservationPose.rotation,
                    Position = mainThreadEvent.ObservationPose.position,
                    Color = Color.gray,
                    IsRemoved = false,
                };
                Observations.Add(mainThreadEvent.NewKeyObservationId, observation);
            }
        }

        public void Update(RemovalEvent removalEvent)
        {
            Debug.Log(removalEvent.EventType.ToString());
            var oldPointsStorage = m_inversePointsHistory.Peek();
            var oldObservationsStorage = m_inverseObservationsHistory.Peek();

            for (int i = 0; i < removalEvent.RemovedCount; ++i)
            {
                if (removalEvent.EventType == SlamEventType.LMPointsRemoval)
                {
                    oldPointsStorage.Add(removalEvent.RemovedIds[i], PointCloud[removalEvent.RemovedIds[i]].Clone());
                    Debug.Log("Added in points history successful");
                    PointCloud[removalEvent.RemovedIds[i]].IsRemoved = true;
                }
                else if (removalEvent.EventType == SlamEventType.LMObservationRemoval)
                {
                    oldObservationsStorage.Add(removalEvent.RemovedIds[i], Observations[removalEvent.RemovedIds[i]].Clone());
                    Debug.Log("Added in observation history successful");
                    Observations[removalEvent.RemovedIds[i]].IsRemoved = true;
                }
            }
        }

        public void Update(PointsFusionEvent lMPointsFusionEvent)
        {
            var oldPointsStorage = m_inversePointsHistory.Peek();

            var maxID = PointCloud.Max(kv => kv.Key);
            Debug.Log(String.Format("FUSION EVENT: Max ID: {0}", maxID));
            // TODO: учесть тип события
            for (int i = 0; i < lMPointsFusionEvent.FusedMapPointsCount; ++i)
            {
                if (lMPointsFusionEvent.ReplacedMapPointsIds[i] == -1 || lMPointsFusionEvent.ReplacingMapPointsIds[i] == -1 ||
                    lMPointsFusionEvent.ReplacedMapPointsIds[i] > maxID || lMPointsFusionEvent.ReplacingMapPointsIds[i] > maxID)
                {
                    continue;
                }
                Debug.Log(String.Format("Fusion: Keys: ReplacedMapPointsId = {0}; ReplacingMapPointsId = {1}", lMPointsFusionEvent.ReplacedMapPointsIds[i], lMPointsFusionEvent.ReplacingMapPointsIds[i]));
                if (!oldPointsStorage.ContainsKey(lMPointsFusionEvent.ReplacedMapPointsIds[i]))
                    oldPointsStorage.Add(lMPointsFusionEvent.ReplacedMapPointsIds[i], PointCloud[lMPointsFusionEvent.ReplacedMapPointsIds[i]].Clone());
                Debug.Log("Added in points history successful (replaced)");
                if (!oldPointsStorage.ContainsKey(lMPointsFusionEvent.ReplacingMapPointsIds[i]))
                    oldPointsStorage.Add(lMPointsFusionEvent.ReplacingMapPointsIds[i], PointCloud[lMPointsFusionEvent.ReplacingMapPointsIds[i]].Clone());
                Debug.Log("Added in points history successful (replacing)");
                PointCloud[lMPointsFusionEvent.ReplacedMapPointsIds[i]].Color = Color.blue;
                PointCloud[lMPointsFusionEvent.ReplacingMapPointsIds[i]].Color = Color.cyan;
            }
        }

        public void Update(MapModificationEvent mapModificationEvent)
        {
            var oldPointsStorage = m_inversePointsHistory.Peek();
            var oldObservationsStorage = m_inverseObservationsHistory.Peek();

            for (int i = 0; i < mapModificationEvent.MovedPtsCount; ++i)
            {
                oldPointsStorage.Add(mapModificationEvent.MovedPtsIds[i], PointCloud[mapModificationEvent.MovedPtsIds[i]].Clone());
                Debug.Log("Added in points history successful");
                PointCloud[mapModificationEvent.MovedPtsIds[i]].Position += mapModificationEvent.RelativeOffsetsOfMovedPts[i];
            }
            for (int i = 0; i < mapModificationEvent.MovedObservationsCount; ++i)
            {
                SlamObservation observation = Observations[mapModificationEvent.MovedObservationsIds[i]];
                oldObservationsStorage.Add(mapModificationEvent.MovedObservationsIds[i], observation.Clone());
                Debug.Log("Added in observation history successful");
                Matrix4x4 currentObservationHomography = Matrix4x4.TRS(observation.Position, observation.Orientation.normalized, Vector3.one);
                Matrix4x4 relativeObservationHomography = Matrix4x4.TRS(
                    mapModificationEvent.RelativeOffsetsOfMovedObservations[i].position,
                    mapModificationEvent.RelativeOffsetsOfMovedObservations[i].rotation.normalized, 
                    Vector3.one);
                Matrix4x4 resultHomography;
                resultHomography = currentObservationHomography * relativeObservationHomography;
                observation.Position = resultHomography.GetColumn(3);
                observation.Orientation = Quaternion.LookRotation(resultHomography.GetColumn(2), resultHomography.GetColumn(1)).normalized;
            }
        }

        public void Update(LoopClosingTryEvent loopClosingTryEvent)
        {
            var oldObservationsStorage = m_inverseObservationsHistory.Peek();
            for (int i = 0; i < loopClosingTryEvent.CandidatesCount; ++i)
            {
                oldObservationsStorage.Add(loopClosingTryEvent.CandidatesIds[i], Observations[loopClosingTryEvent.CandidatesIds[i]].Clone());
                Debug.Log("Added in observation history successful");
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics1 = loopClosingTryEvent.LoopClosingAttempts[i, 0];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics2 = loopClosingTryEvent.LoopClosingAttempts[i, 1];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics3 = loopClosingTryEvent.LoopClosingAttempts[i, 2];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics4 = loopClosingTryEvent.LoopClosingAttempts[i, 3];
            }
        }
    }
}
