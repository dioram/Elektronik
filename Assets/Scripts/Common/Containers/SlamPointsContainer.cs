using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamPointsContainer : ICloudObjectsContainer<SlamPoint>
    {
        private readonly BatchedDictionary<SlamPoint> m_points;
        //private readonly SortedDictionary<int, SlamPoint> m_points;
        private readonly IFastPointsCloud m_pointsCloud;

        public int Count => m_points.Count;

        public bool IsReadOnly => false;

        public SlamPointsContainer(IFastPointsCloud cloud)
        {
            m_points = new BatchedDictionary<SlamPoint>();
            //m_points = new SortedDictionary<int, SlamPoint>();
            m_pointsCloud = cloud;
        }

        public void Add(SlamPoint point)
        {
            m_pointsCloud.Set(new CloudPoint(point.id, point.position, point.color));
            m_points.Add(point.id, point);
        }

        public void Add(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
                m_points[pt.id] = pt;
            m_pointsCloud.Set(points.Select(p => new CloudPoint(p.id, p.position, p.color)));
        }

        public void Update(SlamPoint point)
        {
            SlamPoint currentPoint = m_points[point.id];
            currentPoint.position = point.position;
            currentPoint.color = point.color;
            m_points[point.id] = currentPoint;
            m_pointsCloud.Set(new CloudPoint(point.id, point.position, point.color));
        }

        public void Update(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
            {
                SlamPoint currentPoint = m_points[pt.id];
                currentPoint.position = pt.position;
                currentPoint.color = pt.color;
                m_points[pt.id] = currentPoint;
            }
            m_pointsCloud.Set(points.Select(p => new CloudPoint(p.id, p.position, p.color)));
        }

        public bool Remove(int pointId)
        {
            m_pointsCloud.Remove(pointId);
            return m_points.Remove(pointId);
        }

        public void RemoveAt(int pointId) => Remove(pointId);

        public bool Remove(SlamPoint point) => Remove(point.id);

        public void Remove(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
                m_points.Remove(pt.id);
            m_pointsCloud.Remove(points.Select(p => p.id));
        }

        public void Clear()
        {
            m_points.Clear();
            m_pointsCloud.Clear();
        }

        public IList<SlamPoint> GetAll() => m_points.Values.ToList();

        public SlamPoint this[SlamPoint obj]
        {
            get => this[obj.id];
            set => this[obj.id] = value;
        }

        public SlamPoint this[int id]
        {
            get => m_points[id];
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        public bool Contains(int pointId) => m_points.ContainsKey(pointId);

        public bool Contains(SlamPoint point) => Contains(point.id);

        public bool TryGet(int idx, out SlamPoint current)
        {
            current = new SlamPoint();
            if (Contains(idx))
            {
                current = this[idx];
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGet(SlamPoint point, out SlamPoint current) => TryGet(point.id, out current);

        public IEnumerator<SlamPoint> GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        public bool TryGetAsPoint(SlamPoint obj, out SlamPoint point) => TryGet(obj, out point);

        public bool TryGetAsPoint(int idx, out SlamPoint point) => TryGet(idx, out point);

        public int IndexOf(SlamPoint item) => item.id;

        public void Insert(int index, SlamPoint item) => Add(item);

        public void CopyTo(SlamPoint[] array, int arrayIndex)
            => m_points.Values.CopyTo(array, arrayIndex);
    }
}
