using Elektronik.Common;
using Elektronik.Offline.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    [RequireComponent(typeof(FastPointCloud))]
    public class SlamPointCloudManager : MonoBehaviour
    {
        private FastPointCloud m_pointCloud;
        private Stack<Dictionary<int, SlamPoint>> m_inversePointsHistory;
        private Stack<Dictionary<int, SlamObservation>> m_inverseObservationsHistory;
        private EventFilePlayer m_eventFilePlayer;
        private bool m_play = true;

        void Awake()
        {
            Debug.Log("Analyzing...");
            var events = EventReader.AnalyzeFile(FileModeSettings.Path);
            Debug.Log("Analyzed!");

            m_inverseObservationsHistory = new Stack<Dictionary<int, SlamObservation>>();
            m_inversePointsHistory = new Stack<Dictionary<int, SlamPoint>>();

            m_eventFilePlayer = new EventFilePlayer(events);
        }

        private void Start()
        {
            m_pointCloud = GetComponent<FastPointCloud>();
        }

        public void Play()
        {
            m_play = true;
        }

        public void Pause()
        {
            m_play = false;
        }

        public void Stop()
        {
            m_play = false;
            m_eventFilePlayer.SetPosition(0);
        }

        public void Back()
        {
            Pause();
            m_eventFilePlayer.PrevKeyEvent();
        }

        public void Forward()
        {
            Pause();
            m_eventFilePlayer.NextKeyEvent();
        }

        private void FixedUpdate()
        {
            if (m_play)
            {
                UpdateCloud(m_eventFilePlayer.NextEvent(), false);
                m_pointCloud.Repaint();
                if (m_eventFilePlayer.EndOfFile)
                {
                    Pause();
                }
            }
        }

        public void UpdateCloud(ISlamEvent slamEvent, bool isInversed = false)
        {
            if (isInversed)
            {
                var inversePoints = m_inversePointsHistory.Pop();
                var inverseObservations = m_inverseObservationsHistory.Pop();
                foreach (var point in inversePoints)
                {
                    m_pointCloud.SetPoint(point.Key, point.Value.Position, point.Value.Color);
                }
                foreach (var observation in inverseObservations)
                {
                    //Observations[observation.Key] = observation.Value;
                }
                return;
            }

            m_inversePointsHistory.Push(new Dictionary<int, SlamPoint>());
            m_inverseObservationsHistory.Push(new Dictionary<int, SlamObservation>());

            switch (slamEvent.EventType)
            {
                case SlamEventType.MainThreadEvent:
                    {
                        UpdateCloud(slamEvent as MainThreadEvent);
                        break;
                    }
                case SlamEventType.LMPointsRemoval:
                    {
                        UpdateCloud(slamEvent as RemovalEvent);
                        break;
                    }
                case SlamEventType.LMPointsFusion:
                    {
                        UpdateCloud(slamEvent as PointsFusionEvent);
                        break;
                    }
                case SlamEventType.LMObservationRemoval:
                    {
                        UpdateCloud(slamEvent as RemovalEvent);
                        break;
                    }
                case SlamEventType.LMLBA:
                    {
                        UpdateCloud(slamEvent as MapModificationEvent);
                        break;
                    }
                case SlamEventType.LCLoopClosingTry:
                    {
                        UpdateCloud(slamEvent as LoopClosingTryEvent);
                        break;
                    }
                default:
                    break;
            }
        }

        public void UpdateCloud(MainThreadEvent mainThreadEvent)
        {
            var oldPointsStorage = m_inversePointsHistory.Peek();

            int[] newIds = mainThreadEvent.NewPointsIds;
            Vector3[] newCoords = mainThreadEvent.NewPointsCoordinates;
            Color[] newPointsColors = Enumerable.Repeat(Color.blue, (int)mainThreadEvent.NewPointsCount).ToArray();
            m_pointCloud.SetPoints(newIds, newCoords, newPointsColors);
            Debug.Log("New points added");

            for (int i = 0; i < mainThreadEvent.RecalcPointsCount; ++i)
            {
                //oldPointsStorage.Add(mainThreadEvent.RecalcPointsIds[i], PointCloud[mainThreadEvent.RecalcPointsIds[i]].Clone());
                Debug.Log("Added in points history successful");
                m_pointCloud.SetPointColor(mainThreadEvent.RecalcPointsIds[i], new Color32(0xf4, 0xb3, 0x42, 0xff));
            }
            Debug.Log("Recalc points colored");
            for (int i = 0; i < mainThreadEvent.RecognizedPointsCount; ++i)
            {
                //oldPointsStorage.Add(mainThreadEvent.RecognizedPointsIds[i], PointCloud[mainThreadEvent.RecognizedPointsIds[i]].Clone());
                Debug.Log("Added in points history successful");
                m_pointCloud.SetPointColor(mainThreadEvent.RecognizedPointsIds[i], new Color32(0xf4, 0xb3, 0x42, 0xff));
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
                //Observations.Add(mainThreadEvent.NewKeyObservationId, observation);
            }
        }

        public void UpdateCloud(RemovalEvent removalEvent)
        {
            Debug.Log(removalEvent.EventType.ToString());
            var oldPointsStorage = m_inversePointsHistory.Peek();
            var oldObservationsStorage = m_inverseObservationsHistory.Peek();

            for (int i = 0; i < removalEvent.RemovedCount; ++i)
            {
                if (removalEvent.EventType == SlamEventType.LMPointsRemoval)
                {
                    //oldPointsStorage.Add(removalEvent.RemovedIds[i], PointCloud[removalEvent.RemovedIds[i]].Clone());
                    Debug.Log("Added in points history successful");
                    m_pointCloud.SetPointColor(removalEvent.RemovedIds[i], Color.red);
                }
                else if (removalEvent.EventType == SlamEventType.LMObservationRemoval)
                {
                    //oldObservationsStorage.Add(removalEvent.RemovedIds[i], Observations[removalEvent.RemovedIds[i]].Clone());
                    Debug.Log("Added in observation history successful");
                    //Observations[removalEvent.RemovedIds[i]].IsRemoved = true;
                }
            }
        }

        public void UpdateCloud(PointsFusionEvent lMPointsFusionEvent)
        {
            var oldPointsStorage = m_inversePointsHistory.Peek();

            //var maxID = PointCloud.Max(kv => kv.Key);
            //Debug.Log(String.Format("FUSION EVENT: Max ID: {0}", maxID));
            // TODO: учесть тип события
            for (int i = 0; i < lMPointsFusionEvent.FusedMapPointsCount; ++i)
            {
                if (lMPointsFusionEvent.ReplacedMapPointsIds[i] == -1 || lMPointsFusionEvent.ReplacingMapPointsIds[i] == -1 /*||
                    lMPointsFusionEvent.ReplacedMapPointsIds[i] > maxID || lMPointsFusionEvent.ReplacingMapPointsIds[i] > maxID*/)
                {
                    continue;
                }
                Debug.Log(String.Format("Fusion: Keys: ReplacedMapPointsId = {0}; ReplacingMapPointsId = {1}", lMPointsFusionEvent.ReplacedMapPointsIds[i], lMPointsFusionEvent.ReplacingMapPointsIds[i]));
                /*if (!oldPointsStorage.ContainsKey(lMPointsFusionEvent.ReplacedMapPointsIds[i]))
                    oldPointsStorage.Add(lMPointsFusionEvent.ReplacedMapPointsIds[i], PointCloud[lMPointsFusionEvent.ReplacedMapPointsIds[i]].Clone());
                Debug.Log("Added in points history successful (replaced)");
                if (!oldPointsStorage.ContainsKey(lMPointsFusionEvent.ReplacingMapPointsIds[i]))
                    oldPointsStorage.Add(lMPointsFusionEvent.ReplacingMapPointsIds[i], PointCloud[lMPointsFusionEvent.ReplacingMapPointsIds[i]].Clone());
                Debug.Log("Added in points history successful (replacing)");*/

                m_pointCloud.SetPointColor(lMPointsFusionEvent.ReplacedMapPointsIds[i], new Color32(0xff, 0x00, 0xff, 0xff));
                m_pointCloud.SetPointColor(lMPointsFusionEvent.ReplacingMapPointsIds[i], new Color32(0xff, 0x00, 0xff, 0xff));
            }
        }

        public void UpdateCloud(MapModificationEvent mapModificationEvent)
        {
            var oldPointsStorage = m_inversePointsHistory.Peek();
            var oldObservationsStorage = m_inverseObservationsHistory.Peek();

            for (int i = 0; i < mapModificationEvent.MovedPtsCount; ++i)
            {
                //oldPointsStorage.Add(mapModificationEvent.MovedPtsIds[i], PointCloud[mapModificationEvent.MovedPtsIds[i]].Clone());
                Debug.Log("Added in points history successful");
                Vector3 position;
                Color color;
                int pointIdx = mapModificationEvent.MovedPtsIds[i];
                m_pointCloud.GetPoint(pointIdx, out position, out color);
                position += mapModificationEvent.RelativeOffsetsOfMovedPts[i];
                m_pointCloud.SetPointPosition(pointIdx, position);
            }
            /*for (int i = 0; i < mapModificationEvent.MovedObservationsCount; ++i)
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
            }*/
        }

        public void UpdateCloud(LoopClosingTryEvent loopClosingTryEvent)
        {
            /*var oldObservationsStorage = m_inverseObservationsHistory.Peek();
            for (int i = 0; i < loopClosingTryEvent.CandidatesCount; ++i)
            {
                oldObservationsStorage.Add(loopClosingTryEvent.CandidatesIds[i], Observations[loopClosingTryEvent.CandidatesIds[i]].Clone());
                Debug.Log("Added in observation history successful");
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics1 = loopClosingTryEvent.LoopClosingAttempts[i, 0];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics2 = loopClosingTryEvent.LoopClosingAttempts[i, 1];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics3 = loopClosingTryEvent.LoopClosingAttempts[i, 2];
                Observations[loopClosingTryEvent.CandidatesIds[i]].Statistics4 = loopClosingTryEvent.LoopClosingAttempts[i, 3];
            }*/
        }
    }
}
