using Elektronik.Common.Clouds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamTetrahedronPointsContainer : ISlamContainer<SlamPoint>
    {
        private SortedDictionary<int, SlamPoint> m_points;
        private FastTrianglesCloud m_trianglesCloud;

        private int m_added = 0;
        private int m_removed = 0;
        private int m_diff = 0;

        public SlamTetrahedronPointsContainer(FastTrianglesCloud cloud)
        {
            m_points = new SortedDictionary<int, SlamPoint>();
            m_trianglesCloud = cloud;
        }

        public int Add(SlamPoint point)
        {
            ++m_diff;
            ++m_added;
            m_trianglesCloud.SetTetrahedron(point.id, Matrix4x4.Translate(point.position), point.color);
            Debug.AssertFormat(!m_points.ContainsKey(point.id), "Point with id {0} already in dictionary!", point.id);
            m_points.Add(point.id, point);
            return point.id;
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
            SlamPoint current = m_points[point.id];
            Matrix4x4 to = Matrix4x4.Translate(point.position);
            current.position = point.position;
            current.color = point.color;
            m_points[point.id] = current;
            m_trianglesCloud.SetTetrahedron(point.id, to, point.color);
        }

        public void ChangeColor(SlamPoint point)
        {
            //Debug.LogFormat("[Change color] point {0} color: {1}", point.id, point.color);
            Debug.AssertFormat(m_points.ContainsKey(point.id), "[Change color] Container doesn't contain point with id {0}", point.id);
            m_trianglesCloud.SetTetrahedron(point.id, point.color);
            SlamPoint current = m_points[point.id];
            current.color = point.color;
            m_points[point.id] = current;
        }

        public void Remove(int pointId)
        {
            --m_diff;
            ++m_removed;
            //Debug.LogFormat("Removing point {0}", pointId);
            Debug.AssertFormat(m_points.ContainsKey(pointId), "[Remove] Container doesn't contain point with id {0}", pointId);
            m_trianglesCloud.SetTetrahedron(pointId, Matrix4x4.identity, new Color(0, 0, 0, 0));
            m_points.Remove(pointId);
        }

        public void Remove(SlamPoint point)
        {
            Remove(point.id);
        }

        public void Clear()
        {
            int[] pointsIds = m_points.Keys.ToArray();
            for (int i = 0; i < pointsIds.Length; ++i)
            {
                Remove(pointsIds[i]);
            }
            m_points.Clear();
            m_trianglesCloud.Clear();
            Repaint();

            Debug.LogFormat("[Clear] Added points: {0}; Removed points: {1}; Diff: {2}", m_added, m_removed, m_diff);
            m_added = 0;
            m_removed = 0;
        }

        public SlamPoint[] GetAll()
        {
            return m_points.Select(kv => kv.Value).ToArray();
        }

        public void Set(SlamPoint point)
        {
            SlamPoint buttPlug;
            if (!TryGet(point, out buttPlug))
            {
                Add(point);
            }
            else
            {
                Update(point);
            }
        }

        public SlamPoint Get(int pointId)
        {
            //Debug.AssertFormat(m_points.ContainsKey(pointId), "[Get point] Container doesn't contain point with id {0}", pointId);
            if (!m_points.ContainsKey(pointId))
            {
                Debug.LogWarningFormat("[Get point] Container doesn't contain point with id {0}", pointId);
                return new SlamPoint();
            }

            return m_points[pointId];
        }

        public SlamPoint Get(SlamPoint point)
        {
            return Get(point.id);
        }

        public bool Exists(int pointId)
        {
            //return m_pointCloud.PointExists(pointId);
            return m_points.ContainsKey(pointId);
        }

        public bool Exists(SlamPoint point)
        {
            return Exists(point.id);
        }

        public bool TryGet(SlamPoint point, out SlamPoint current)
        {
            current = new SlamPoint();
            if (m_trianglesCloud.TetrahedronExists(point.id))
            {
                current = Get(point.id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Repaint()
        {
            m_trianglesCloud.Repaint();
        }
    }
}
