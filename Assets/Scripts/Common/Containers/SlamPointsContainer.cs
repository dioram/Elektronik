using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds.V2;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamPointsContainer : MonoBehaviour, ICloudObjectsContainer<SlamPoint>
    {
        public PointCloudRenderer Renderer;
        
        public SlamPoint this[int id]
        {
            get => _points[id];
            set
            {
                if (!TryGet(id, out _)) Add(value); else UpdateItem(value);
            }
        }

        #region Unity events

        private void Start()
        {
            if (Renderer == null)
            {
                Debug.LogWarning($"No renderer set for {name}({GetType()})");
            }
            else
            {
                ItemsAdded += Renderer.OnItemsAdded;
                ItemsUpdated += Renderer.OnItemsUpdated;
                ItemsRemoved += Renderer.OnItemsRemoved;
                ItemsCleared += Renderer.OnItemsCleared;
            }
        }

        #endregion

        #region IContainer implementation

        public event Action<IEnumerable<SlamPoint>> ItemsAdded;
        public event Action<IEnumerable<SlamPoint>> ItemsUpdated;
        public event Action<IEnumerable<int>> ItemsRemoved;
        public event Action ItemsCleared;
        
        public IEnumerator<SlamPoint> GetEnumerator() => _points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _points.Select(kv => kv.Value).GetEnumerator();

        public SlamPoint this[SlamPoint obj]
        {
            get => this[obj.id];
            set => this[obj.id] = value;
        }
        
        public int Count => _points.Count;

        public bool IsReadOnly => false;
        
        public void CopyTo(SlamPoint[] array, int arrayIndex) => _points.Values.CopyTo(array, arrayIndex);
        
        public int IndexOf(SlamPoint item) => item.id;

        public bool Contains(SlamPoint point) => Contains(point.id);
        
        public bool TryGet(SlamPoint point, out SlamPoint current) => TryGet(point.id, out current);
        
        public void Add(SlamPoint point)
        {
            _points.Add(point.id, point);
            ItemsAdded?.Invoke(new []{point});
        }

        public void AddRange(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
                _points.Add(pt.id, pt);
            ItemsAdded?.Invoke(points);
        }
        
        public void Insert(int index, SlamPoint item) => Add(item);

        public void UpdateItem(SlamPoint point)
        {
            SlamPoint currentPoint = _points[point.id];
            currentPoint.position = point.position;
            currentPoint.color = point.color;
            _points[point.id] = currentPoint;
            ItemsUpdated?.Invoke(new []{point});
        }

        public void UpdateItems(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
            {
                SlamPoint currentPoint = _points[pt.id];
                currentPoint.position = pt.position;
                currentPoint.color = pt.color;
                _points[pt.id] = currentPoint;
            }
            ItemsUpdated?.Invoke(points);
        }

        public void RemoveAt(int pointId)
        {
            _points.Remove(pointId);
            ItemsRemoved?.Invoke(new []{pointId});
        }

        public bool Remove(SlamPoint point)
        {
            var res = _points.Remove(point.id);
            ItemsRemoved?.Invoke(new []{point.id});
            return res;
        }

        public void Remove(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
                _points.Remove(pt.id);
            ItemsRemoved?.Invoke(points.Select(p => p.id));
        }
        
        public void Clear()
        {
            _points.Clear();
           ItemsCleared?.Invoke();
        }

        #endregion

        #region ICloudObjectsContainer implementation
        
        public bool Contains(int pointId) => _points.ContainsKey(pointId);

        public bool TryGet(int idx, out SlamPoint current)
        {
            current = new SlamPoint();
            if (!Contains(idx)) return false;
            current = this[idx];
            return true;

        }

        public bool TryGetAsPoint(SlamPoint obj, out SlamPoint point) => TryGet(obj, out point);

        public bool TryGetAsPoint(int idx, out SlamPoint point) => TryGet(idx, out point);

        #endregion

        #region Private definitions
        
        private readonly BatchedDictionary<SlamPoint> _points = new BatchedDictionary<SlamPoint>();

        #endregion
    }
}
