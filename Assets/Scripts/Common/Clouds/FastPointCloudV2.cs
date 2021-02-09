using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPointCloudV2 : MonoBehaviour, IFastPointsCloud
    {
        public event Action CloudUpdated;
        private Dictionary<int, CloudPoint> m_points = new Dictionary<int, CloudPoint>();
        
        public void Clear()
        {
            m_points.Clear();
            CloudUpdated?.Invoke();
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

        public void Set(CloudPoint point)
        {
            PureSet(point);
            CloudUpdated?.Invoke();
        }

        public void Set(int idx, Color color)
        {
            CloudPoint point;
            if (Exists(idx))
            {
                point = m_points[idx];
                point.color = color;
            }
            else
            {
                point = new CloudPoint(idx, Vector3.zero, color);
            }
            PureSet(point);
            CloudUpdated?.Invoke();
        }

        public void Set(int idx, Vector3 translation)
        {
            CloudPoint point;
            if (Exists(idx))
            {
                point = m_points[idx];
                point.offset = translation;
            }
            else
            {
                point = new CloudPoint(idx, translation, Color.black);
            }
            PureSet(point);
            CloudUpdated?.Invoke();
        }

        public void Set(IEnumerable<CloudPoint> points)
        {
            foreach (var point in points)
            {
                PureSet(point);
            }
            CloudUpdated?.Invoke();
        }

        private void PureSet(CloudPoint point)
        {
            m_points[point.idx] = point;
        }

        public void Remove(int idx)
        {
            m_points.Remove(idx);
            CloudUpdated?.Invoke();
        }

        public void Remove(IEnumerable<int> pointsIds)
        {
            foreach (var id in pointsIds)
            {
                m_points.Remove(id);
            }
            CloudUpdated?.Invoke();
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}