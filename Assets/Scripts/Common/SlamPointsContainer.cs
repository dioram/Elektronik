using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public class SlamPointsContainer
    {
        private SortedDictionary<int, SlamPoint> m_points;
        private FastPointCloud m_pointCloud;

        public SlamPointsContainer(FastPointCloud cloud)
        {
            m_points = new SortedDictionary<int, SlamPoint>();
            m_pointCloud = cloud;
        }

        public void Add(SlamPoint point)
        {
            m_pointCloud.SetPoint(point.id, point.position, point.color);
            m_points.Add(point.id, point);
        }

        public void AddRange(SlamPoint[] points)
        {
            foreach (var point in points)
            {
                Add(point);
            }
        }

        public void Update(SlamPoint point)
        {
            Debug.AssertFormat(m_points.ContainsKey(point.id), "[Update] Container doesn't contain point with id {0}", point.id);
            m_pointCloud.SetPoint(point.id, point.position, point.color);
            m_points[point.id] = point;
        }

        public void ChangeColor(SlamPoint point)
        {
            Debug.AssertFormat(m_points.ContainsKey(point.id), "[Change color] Container doesn't contain point with id {0}", point.id);
            m_pointCloud.SetPointColor(point.id, point.color);
            SlamPoint current = m_points[point.id];
            current.color = point.color;
            m_points[point.id] = current;
        }

        public void Remove(int pointId)
        {
            Debug.AssertFormat(m_points.ContainsKey(pointId), "[Remove] Container doesn't contain point with id {0}", pointId);
            m_pointCloud.SetPoint(pointId, Vector3.zero, new Color(0, 0, 0, 0));
            m_points.Remove(pointId);
        }

        public void Clear()
        {
            foreach (var pointId in m_points.Keys)
            {
                m_pointCloud.SetPoint(pointId, Vector3.zero, new Color(0, 0, 0, 0));
            }
            m_points.Clear();
            Repaint();
        }

        public SlamPoint[] GetAllSlamPoints()
        {
            return m_points.Select(kv => kv.Value).ToArray();
        }

        public void SetPoint(SlamPoint point)
        {
            SlamPoint buttPlug;
            if (!TryGetPoint(point, out buttPlug))
            {
                Add(point);
            }
            else
            {
                Update(point);
            }
        }

        public SlamPoint GetPoint(int pointId)
        {
            Debug.AssertFormat(m_points.ContainsKey(pointId), "[Get point] Container doesn't contain point with id {0}", pointId);
            return m_points[pointId];
        }

        public bool PointExists(int pointId)
        {
            return m_pointCloud.PointExists(pointId);
        }

        public bool TryGetPoint(SlamPoint point, out SlamPoint current)
        {
            current = new SlamPoint();
            if (m_pointCloud.PointExists(point.id))
            {
                current = GetPoint(point.id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Repaint()
        {
            m_pointCloud.Repaint();
        }
    }
}
