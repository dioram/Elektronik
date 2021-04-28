using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Containers
{
    /// <summary> Contains lines in strict order. </summary>
    public class TrackContainer : IContainer<SlamLine>, ISourceTree, ILookable, IVisible, ISnapshotable
    {
        #region IContainer implementaion

        public IEnumerator<SlamLine> GetEnumerator()
        {
            lock (_lines)
            {
                return _lines.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SlamLine item)
        {
            lock (_lines)
            {
                _lines.Add(item);
            }
            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {item}));
            }
        }

        public void Clear()
        {
            List<int> lines;
            lock (_lines)
            {
                lines = _lines.Select(l => l.Id).ToList();
                _lines.Clear();
            }
            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(lines));
            }
        }

        public bool Contains(SlamLine item)
        {
            lock (_lines)
            {
                return _lines.Contains(item);
            }
        }

        public void CopyTo(SlamLine[] array, int arrayIndex)
        {
            lock (_lines)
            {
                _lines.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(SlamLine item)
        {
            bool res;
            lock (_lines)
            {
                res = _lines.Remove(item);
            }

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {item.Id}));
            }

            return res;
        }

        public int Count
        {
            get
            {
                lock (_lines)
                {
                    return _lines.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public int IndexOf(SlamLine item)
        {
            lock (_lines)
            {
                return _lines.IndexOf(item);
            }
        }

        public void Insert(int index, SlamLine item)
        {
            lock (_lines)
            {
                _lines.Insert(index, item);
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {item}));
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lines)
            {
                _lines.RemoveAt(index);
            }

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {index}));
            }
        }

        public SlamLine this[int index]
        {
            get
            {
                lock (_lines)
                {
                    return _lines[index];
                }
            }
            set
            {
                lock (_lines)
                {
                    _lines[index] = value;
                }

                if (IsVisible)
                {
                    OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {value}));
                }
            }
        }

        public event Action<IContainer<SlamLine>, AddedEventArgs<SlamLine>> OnAdded;

        public event Action<IContainer<SlamLine>, UpdatedEventArgs<SlamLine>> OnUpdated;

        public event Action<IContainer<SlamLine>, RemovedEventArgs> OnRemoved;

        public void AddRange(IEnumerable<SlamLine> items)
        {
            var list = items.ToList();
            lock (_lines)
            {
                _lines.AddRange(list);
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(list));
            }
        }

        public void Remove(IEnumerable<SlamLine> items)
        {
            var list = items.ToList();
            lock (_lines)
            {
                foreach (var item in list)
                {
                    _lines.Remove(item);
                }
            }

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(i => i.Id)));
            }
        }

        public void Update(SlamLine item)
        {
            lock (_lines)
            {
                var index = _lines.FindIndex(l => l.Id == item.Id);
                _lines[index] = item;
            }

            if (IsVisible)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {item}));
            }
        }

        public void Update(IEnumerable<SlamLine> items)
        {
            var list = items.ToList();
            lock (_lines)
            {
                foreach (var item in list)
                {
                    var index = _lines.FindIndex(l => l.Id == item.Id);
                    _lines[index] = item;
                }
            }

            if (IsVisible)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(list));
            }
        }

        #endregion

        #region ISourceTree implementation

        public string DisplayName { get; set; } = "Track";

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

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
            Vector3 pos;
            lock (_lines)
            {
                if (_lines.Count == 0) return (transform.position, transform.rotation);

                pos = _lines.Last().Point2.Position;
            }

            return (pos + (transform.position - pos).normalized, Quaternion.LookRotation(pos - transform.position));
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
                
                if (_isVisible)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                    return;
                }

                List<int> lines;
                lock (_lines)
                {
                    lines = _lines.Select(l => l.Id).ToList();
                }

                OnRemoved?.Invoke(this, new RemovedEventArgs(lines));
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var res = new TrackContainer();
            lock (_lines)
            {
                res.AddRange(_lines);
            }

            return res;
        }

        #endregion

        #region Private definition

        private readonly List<SlamLine> _lines = new List<SlamLine>();
        private bool _isVisible = true;

        #endregion
    }
}