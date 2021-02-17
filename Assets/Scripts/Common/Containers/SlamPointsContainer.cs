using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamPointsContainer : MonoBehaviour, IClearable, ICloudObjectsContainer<SlamPoint>
    {
        public PointCloudRenderer Renderer;

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
            }
        }

        private void OnEnable()
        {
            ItemsAdded?.Invoke(this, this);
        }

        private void OnDisable()
        {
            ItemsRemoved?.Invoke(this, _points.Keys);
        }

        private void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region IContainer implementation

        public event Action<IContainer<SlamPoint>, IEnumerable<SlamPoint>> ItemsAdded;
        public event Action<IContainer<SlamPoint>, IEnumerable<SlamPoint>> ItemsUpdated;
        public event Action<IContainer<SlamPoint>, IEnumerable<int>> ItemsRemoved;
        
        public IEnumerator<SlamPoint> GetEnumerator() => _points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _points.Select(kv => kv.Value).GetEnumerator();

        public SlamPoint this[int id]
        {
            get => _points[id];
            set
            {
                if (!TryGet(id, out _)) Add(value); else UpdateItem(value);
            }
        }
        
        public SlamPoint this[SlamPoint obj]
        {
            get => this[obj.Id];
            set => this[obj.Id] = value;
        }
        
        public int Count => _points.Count;

        public bool IsReadOnly => false;
        
        public void CopyTo(SlamPoint[] array, int arrayIndex) => _points.Values.CopyTo(array, arrayIndex);
        
        public int IndexOf(SlamPoint item) => item.Id;

        public bool Contains(SlamPoint point) => Contains(point.Id);
        
        public bool TryGet(SlamPoint point, out SlamPoint current) => TryGet(point.Id, out current);
        
        public void Add(SlamPoint point)
        {
            _points.Add(point.Id, point);
            ItemsAdded?.Invoke(this, new []{point});
        }

        public void AddRange(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
                _points.Add(pt.Id, pt);
            ItemsAdded?.Invoke(this, points);
        }
        
        public void Insert(int index, SlamPoint item) => Add(item);

        public void UpdateItem(SlamPoint point)
        {
            SlamPoint currentPoint = _points[point.Id];
            currentPoint.Position = point.Position;
            currentPoint.Color = point.Color;
            _points[point.Id] = currentPoint;
            ItemsUpdated?.Invoke(this, new []{point});
        }

        public void UpdateItems(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
            {
                SlamPoint currentPoint = _points[pt.Id];
                currentPoint.Position = pt.Position;
                currentPoint.Color = pt.Color;
                _points[pt.Id] = currentPoint;
            }
            ItemsUpdated?.Invoke(this, points);
        }

        public void RemoveAt(int pointId)
        {
            _points.Remove(pointId);
            ItemsRemoved?.Invoke(this, new []{pointId});
        }

        public bool Remove(SlamPoint point)
        {
            var res = _points.Remove(point.Id);
            ItemsRemoved?.Invoke(this, new []{point.Id});
            return res;
        }

        public void Remove(IEnumerable<SlamPoint> points)
        {
            foreach (var pt in points)
                _points.Remove(pt.Id);
            ItemsRemoved?.Invoke(this, points.Select(p => p.Id));
        }
        
        public void Clear()
        {
            var ids = _points.Keys.ToArray();
            _points.Clear();
            ItemsRemoved?.Invoke(this, ids);
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
