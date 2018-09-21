using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Common;
using Elektronik.Offline.Events;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public class CloudsManager : MonoBehaviour
    {
        FastPointCloud pointCloud;
        SlamObservationsGraph slamObservationsGraph;

        public void UpdatePoints(ISlamEvent slamEvent)
        {
            for (int i = 0; i < slamEvent.Points.Length; ++i)
            {
                SlamPoint point = slamEvent.Points[i];
                Vector3 currentPointPosition;
                Color currentPointColor;
                pointCloud.GetPoint(point.id, out currentPointPosition, out currentPointColor);
                pointCloud.SetPoint(point.id, currentPointPosition + point.position, point.color);
            }
        }

        public void UpdateObservations(ISlamEvent slamEvent)
        {
            for (int i = 0; i < slamEvent.Observations.Length; ++i)
            {
                SlamObservation observation = slamEvent.Observations[i];
                if (!slamObservationsGraph.ObservationExists(observation.id)) // новый observation
                {
                    slamObservationsGraph.AddNewObservation(observation);
                }
                else if (observation.isRemoved == true) // удалённый observation
                {
                    slamObservationsGraph.RemoveObservation(observation.id);
                }
                else if (observation.id == -1) // камера
                {
                    slamObservationsGraph.ReplaceObservation(observation);
                }
                else // перемещение ключевого observation
                {
                    SlamObservation currentObservation = slamObservationsGraph.GetObservationNode(observation.id);
                    Matrix4x4 currentOrientation = Matrix4x4.TRS(currentObservation.position, currentObservation.orientation.normalized, Vector3.one);
                    Matrix4x4 relativeOrientation = Matrix4x4.TRS(observation.position, observation.orientation.normalized, Vector3.one);
                    Matrix4x4 newOrientation = currentOrientation * relativeOrientation.inverse;
                    observation.position = newOrientation.GetColumn(3);
                    observation.orientation = Quaternion.LookRotation(newOrientation.GetColumn(2), newOrientation.GetColumn(1));
                    slamObservationsGraph.ReplaceObservation(observation);
                }
            }
        }

        public SlamObservation[] GetCurrentObservations(int[] ids)
        {
            var observations = ids.Select(id =>
            {
                SlamObservation observation;
                if (slamObservationsGraph.ObservationExists(id))
                {
                    observation = slamObservationsGraph.GetObservationNode(id);
                }
                else
                {
                    observation = new SlamObservation()
                    {
                        id = id,
                        isRemoved = true,
                    };
                }
                return observation;
            }).ToArray();
            return observations;
        }

        public SlamPoint[] GetCurrentPoints(int[] ids)
        {
            var points = ids.Select(id => 
            {
                Vector3 position;
                Color color;
                pointCloud.GetPoint(id, out position, out color);
                SlamPoint point = new SlamPoint()
                {
                    color = color,
                    id = id,
                    position = position,
                    isRemoved = false,
                };
                return point;
            }).ToArray();
            return points;
        }

    }
}
