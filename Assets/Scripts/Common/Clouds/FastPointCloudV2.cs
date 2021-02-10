using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPointCloudV2 : MonoBehaviour, IFastPointsCloud
    {
        public event Action<IList<CloudPoint>> PointsAdded;
        
        public event Action<IList<CloudPoint>> PointsUpdated;
        
        public event Action<IList<int>> PointsRemoved;

        public event Action PointsCleared; 

        private Dictionary<int, CloudPoint> m_points = new Dictionary<int, CloudPoint>();

        public int Count => m_points.Count();

        public void Clear()
        {
            m_points.Clear();
            PointsCleared?.Invoke();
        }

        public bool Exists(int idx)
        {
            return m_points.ContainsKey(idx);
        }

        public CloudPoint Get(int idx)
        {
            return m_points[idx];
        }

        public IEnumerable<CloudPoint> GetAll()
        {
            return m_points.Values;
        }

        public void Add(CloudPoint point)
        {
            m_points.Add(point.idx, point);
            PointsAdded?.Invoke(new []{point});
        }

        public void Add(IEnumerable<CloudPoint> points)
        {
            foreach (var point in points)
            {
                m_points.Add(point.idx, point);
            }
            PointsAdded?.Invoke(points.ToList());
        }

        public void UpdatePoint(CloudPoint point)
        {
            m_points[point.idx] = point;
            PointsUpdated?.Invoke(new []{point});
        }

        public void UpdatePoints(IEnumerable<CloudPoint> points)
        {
            foreach (var point in points)
            {
                m_points[point.idx] = point;
            }
            PointsUpdated?.Invoke(points.ToList());
        }

        private void PureSet(CloudPoint point)
        {
            m_points[point.idx] = point;
        }

        public void Remove(int idx)
        {
            m_points.Remove(idx);
            PointsRemoved?.Invoke(new []{idx});
        }

        public void Remove(IEnumerable<int> pointsIds)
        {
            foreach (var id in pointsIds)
            {
                m_points.Remove(id);
            }
            PointsRemoved?.Invoke(pointsIds.ToList());
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}