using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Containers
{
    /// <summary> Contains lines in strict order. </summary>
    public class TrackContainer : IContainer<SlamLine>, ISourceTree, ILookable
    {
        #region IContainer implementaion

        public IEnumerator<SlamLine> GetEnumerator() => _lines.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SlamLine item)
        {
            _lines.Add(item);
            if (IsActive)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new []{item}));
            }
        }

        public void Clear()
        {
            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(_lines.Select(l => l.Id)));
            }
            _lines.Clear();
        }

        public bool Contains(SlamLine item) => _lines.Contains(item);

        public void CopyTo(SlamLine[] array, int arrayIndex)
        {
            _lines.CopyTo(array, arrayIndex);
        }

        public bool Remove(SlamLine item)
        {
            var res = _lines.Remove(item);
            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new []{item.Id}));
            }
            return res;
        }

        public int Count => _lines.Count;
        
        public bool IsReadOnly => false;

        public int IndexOf(SlamLine item) => _lines.IndexOf(item);

        public void Insert(int index, SlamLine item)
        {
            _lines.Insert(index, item);
            if (IsActive)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new []{item}));
            }
        }

        public void RemoveAt(int index)
        {
            _lines.RemoveAt(index);
            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new []{index}));
            }
        }

        public SlamLine this[int index]
        {
            get => _lines[index];
            set
            {
                _lines[index] = value;
                if (IsActive)
                {
                    OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new []{value}));
                }
            }
        }

        public event Action<IContainer<SlamLine>, AddedEventArgs<SlamLine>> OnAdded;
        
        public event Action<IContainer<SlamLine>, UpdatedEventArgs<SlamLine>> OnUpdated;
        
        public event Action<IContainer<SlamLine>, RemovedEventArgs> OnRemoved;
        
        public void AddRange(IEnumerable<SlamLine> items)
        {
            _lines.AddRange(items);
            if (IsActive)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(items));
            }
        }

        public void Remove(IEnumerable<SlamLine> items)
        {
            foreach (var item in items)
            {
                _lines.Remove(item);
            }

            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(items.Select(i => i.Id)));
            }
        }

        public void Update(SlamLine item)
        {
            var index = _lines.FindIndex(l => l.Id == item.Id);
            _lines[index] = item;
            if (IsActive)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new []{item}));
            }
        }

        public void Update(IEnumerable<SlamLine> items)
        {
            foreach (var item in items)
            {
                var index = _lines.FindIndex(l => l.Id == item.Id);
                _lines[index] = item;
            }

            if (IsActive)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(items));
            }
        }

        #endregion

        #region IContainerTree implementation

        public string DisplayName { get; set; } = "Track";

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();
        
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                if (_isActive) OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                else OnRemoved?.Invoke(this, new RemovedEventArgs(_lines.Select(l => l.Id)));
            }
        }

        public void SetRenderer(object renderer)
        {
            if (renderer is ICloudRenderer<SlamLine> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                }
            }
        }

        #endregion

        #region ILookable implementaion

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            if (_lines.Count == 0) return (transform.position, transform.rotation);

            var pos = _lines.Last().Point2.Position;
            return (pos + (transform.position - pos).normalized, Quaternion.LookRotation(pos - transform.position));
        }

        #endregion

        #region Private definition

        private readonly List<SlamLine> _lines = new List<SlamLine>();
        private bool _isActive = true;

        #endregion
    }
}