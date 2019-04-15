using Elektronik.Common.Clouds;
using Elektronik.Common.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamPointsContainer : ICloudObjectsContainer<SlamPoint>
    {
        private readonly SortedDictionary<int, SlamPoint> m_points;
        private readonly IFastPointsCloud m_pointsCloud;

        private int m_added = 0;
        private int m_removed = 0;
        private int m_diff = 0;

        

        public SlamPointsContainer(IFastPointsCloud cloud)
        {
            m_points = new SortedDictionary<int, SlamPoint>();
            m_pointsCloud = cloud;
        }

        public int Add(SlamPoint point)
        {
            Debug.AssertFormat(
                !m_points.ContainsKey(point.id), 
                "[SlamPointsContainer.Add] Point with id {0} already in dictionary!", point.id);
            ++m_diff;
            ++m_added;
            m_pointsCloud.Set(point.id, Matrix4x4.Translate(point.position), point.color);
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
            Debug.AssertFormat(
                m_points.ContainsKey(point.id), 
                "[SlamPointsContainer.Update] Container doesn't contain point with id {0}", point.id);
            Matrix4x4 to = Matrix4x4.Translate(point.position);
            SlamPoint currentPoint = m_points[point.id];
            currentPoint.position = point.position;
            currentPoint.color = point.color;
            m_points[point.id] = currentPoint;
            m_pointsCloud.Set(point.id, to, point.color);
        }

        public void ChangeColor(SlamPoint point)
        {
            Debug.AssertFormat(
                m_points.ContainsKey(point.id),
                "[SlamPointsContainer.ChangeColor] Container doesn't contain point with id {0}", point.id);
            m_pointsCloud.Set(point.id, point.color);
            SlamPoint currentPoint = m_points[point.id];
            currentPoint.color = point.color;
            m_points[point.id] = currentPoint;
        }

        public void Remove(int pointId)
        {
            --m_diff;
            ++m_removed;
            Debug.AssertFormat(
                m_points.ContainsKey(pointId), 
                "[SlamPointsContainer.Remove] Container doesn't contain point with id {0}", pointId);
            m_pointsCloud.Set(pointId, Matrix4x4.identity, new Color(0, 0, 0, 0));
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
            m_pointsCloud.Clear();
            Repaint();

            Debug.LogFormat(
                "[SlamPointsContainer.Clear] Added points: {0}; Removed points: {1}; Diff: {2}", m_added, m_removed, m_diff);
            m_added = 0;
            m_removed = 0;
        }

        public SlamPoint[] GetAll()
        {
            return m_points.Select(kv => kv.Value).ToArray();
        }

        public SlamPoint this[SlamPoint obj]
        {
            get => this[obj.id];
            set => this[obj.id] = value;
        }
        public SlamPoint this[int id]
        {
            get
            {
                Debug.AssertFormat(
                    m_points.ContainsKey(id),
                    "[SlamPointsContainer.Get] Container doesn't contain point with id {0}", id);
                return m_points[id];
            }
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        public bool Exists(int pointId)
        {
            return m_points.ContainsKey(pointId);
        }

        public bool Exists(SlamPoint point)
        {
            return Exists(point.id);
        }
        public bool TryGet(int idx, out SlamPoint current)
        {
            current = new SlamPoint();
            if (m_pointsCloud.Exists(idx))
            {
                current = this[idx];
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool TryGet(SlamPoint point, out SlamPoint current)
        {
            return TryGet(point.id, out current);
        }

        public void Repaint()
        {
            m_pointsCloud.Repaint();
        }

        public IEnumerator<SlamPoint> GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        
    }
}
