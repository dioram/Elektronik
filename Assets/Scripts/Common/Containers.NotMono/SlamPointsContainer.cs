using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Containers.NotMono
{
    public class SlamPointsContainer : IClearable, IContainer<SlamPoint>, IContainerTree
    {
        public PointCloudRenderer Renderer;
        

        #region IContainer implementation

        public event Action<IContainer<SlamPoint>, AddedEventArgs<SlamPoint>> OnAdded;
        public event Action<IContainer<SlamPoint>, UpdatedEventArgs<SlamPoint>> OnUpdated;
        public event Action<IContainer<SlamPoint>, RemovedEventArgs> OnRemoved;
        
        public IEnumerator<SlamPoint> GetEnumerator() => _points.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _points.Select(kv => kv.Value).GetEnumerator();

        public SlamPoint this[int id]
        {
            get => _points[id];
            set
            {
                if (_points.ContainsKey(id))
                {
                    UpdateItem(value);
                }
                else
                {
                    Add(value);
                }
            }
        }
        
        public int Count => _points.Count;

        public bool IsReadOnly => false;
        
        public void CopyTo(SlamPoint[] array, int arrayIndex) => _points.Values.CopyTo(array, arrayIndex);
        
        public int IndexOf(SlamPoint item) => item.Id;

        public bool Contains(SlamPoint point) => _points.ContainsKey(point.Id);

        public void Add(SlamPoint point)
        {
            _points.Add(point.Id, point);
            OnAdded?.Invoke(this, new AddedEventArgs<SlamPoint>(new []{point}));
        }

        public void AddRange(IEnumerable<SlamPoint> items)
        {
            foreach (var pt in items)
                _points.Add(pt.Id, pt);
            OnAdded?.Invoke(this, new AddedEventArgs<SlamPoint>(items));
        }
        
        public void Insert(int index, SlamPoint item) => Add(item);

        public void UpdateItem(SlamPoint item)
        {
            SlamPoint currentPoint = _points[item.Id];
            currentPoint.Position = item.Position;
            currentPoint.Color = item.Color;
            _points[item.Id] = currentPoint;
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamPoint>(new []{item}));
        }

        public void UpdateItems(IEnumerable<SlamPoint> items)
        {
            foreach (var pt in items)
            {
                SlamPoint currentPoint = _points[pt.Id];
                currentPoint.Position = pt.Position;
                currentPoint.Color = pt.Color;
                _points[pt.Id] = currentPoint;
            }
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamPoint>(items));
        }

        public void RemoveAt(int pointId)
        {
            _points.Remove(pointId);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new []{pointId}));
        }

        public bool Remove(SlamPoint point)
        {
            var res = _points.Remove(point.Id);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new []{point.Id}));
            return res;
        }

        public void Remove(IEnumerable<SlamPoint> items)
        {
            foreach (var pt in items)
                _points.Remove(pt.Id);
            OnRemoved?.Invoke(this, new RemovedEventArgs(items.Select(p => p.Id)));
        }

        public void Clear()
        {
            var ids = _points.Keys.ToArray();
            _points.Clear();
            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        #endregion

        #region IContainerTree implementations

        public string DisplayName => "Points";
        
        public IContainerTree[] Children { get; }

        public void SetActive(bool active)
        {
            if (active)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamPoint>(this));
            }
            else
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(_points.Keys));
            }
        }

        #endregion

        #region Private definitions
        
        private readonly BatchedDictionary<SlamPoint> _points = new BatchedDictionary<SlamPoint>();

        #endregion
    }
}
