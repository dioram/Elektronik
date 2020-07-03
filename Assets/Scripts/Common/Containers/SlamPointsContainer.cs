using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using System;
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

        public int Count => m_points.Count;

        public SlamPointsContainer(IFastPointsCloud cloud)
        {
            m_points = new SortedDictionary<int, SlamPoint>();
            m_pointsCloud = cloud;
        }

        public void Add(SlamPoint point)
        {
            if (m_points.ContainsKey(point.id))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Add] Point with id {point.id} already in dictionary!");
            m_pointsCloud.Set(new CloudPoint(point.id, Matrix4x4.Translate(point.position), point.color));
            m_points.Add(point.id, point);
        }

        public void Add(IEnumerable<SlamPoint> points)
        {
            foreach (var point in points)
            {
                if (m_points.ContainsKey(point.id))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Add] Point with id {point.id} already in dictionary!");
                m_points.Add(point.id, point);
            }
            m_pointsCloud.Set(points.Select(p => new CloudPoint(p.id, Matrix4x4.Translate(p.position), p.color)));
        }

        public void Update(SlamPoint point)
        {
            if (!m_points.ContainsKey(point.id))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Update] Container doesn't contain point with id {point.id}");
            Matrix4x4 translationMat = Matrix4x4.Translate(point.position);
            SlamPoint currentPoint = m_points[point.id];
            currentPoint.position = point.position;
            currentPoint.color = point.color;
            m_points[point.id] = currentPoint;
            m_pointsCloud.Set(new CloudPoint(point.id, translationMat, point.color));
        }

        public void Update(IEnumerable<SlamPoint> points)
        {
            foreach (var point in points)
            {
                if (!m_points.ContainsKey(point.id))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Update] Container doesn't contain point with id {point.id}");
                SlamPoint currentPoint = m_points[point.id];
                currentPoint.position = point.position;
                currentPoint.color = point.color;
                m_points[point.id] = currentPoint;
            }
            m_pointsCloud.Set(points.Select(p => new CloudPoint(p.id, Matrix4x4.Translate(p.position), p.color)));
        }

        public void Remove(int pointId)
        {
            if (!m_points.ContainsKey(pointId))
                throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Remove] Container doesn't contain point with id {pointId}");
            m_pointsCloud.Set(new CloudPoint(pointId, Matrix4x4.identity, new Color(0, 0, 0, 0)));
            m_points.Remove(pointId);
        }
        
        public void Remove(SlamPoint point) => Remove(point.id);

        public void Remove(IEnumerable<SlamPoint> points)
        {
            foreach (var point in points)
            {
                if (!m_points.ContainsKey(point.id))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Remove] Container doesn't contain point with id {point.id}");
                m_points.Remove(point.id);
            }
            m_pointsCloud.Set(points.Select(p => new CloudPoint(p.id, Matrix4x4.identity, new Color(0, 0, 0, 0))));
        }

        public void Clear()
        {
            m_points.Clear();
            m_pointsCloud.Clear();
        }

        public SlamPoint[] GetAll() => m_points.Select(kv => kv.Value).ToArray();

        public SlamPoint this[SlamPoint obj]
        {
            get => this[obj.id];
            set => this[obj.id] = value;
        }

        public SlamPoint this[int id]
        {
            get
            {
                if (!m_points.ContainsKey(id))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Get] Container doesn't contain point with id {id}");
                return m_points[id];
            }
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        public bool Exists(int pointId) => m_points.ContainsKey(pointId);

        public bool Exists(SlamPoint point) => Exists(point.id);

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

        public bool TryGet(SlamPoint point, out SlamPoint current) => TryGet(point.id, out current);

        public IEnumerator<SlamPoint> GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_points.Select(kv => kv.Value).GetEnumerator();

        public bool TryGetAsPoint(SlamPoint obj, out SlamPoint point) => TryGet(obj, out point);

        public bool TryGetAsPoint(int idx, out SlamPoint point) => TryGet(idx, out point);
    }
}
