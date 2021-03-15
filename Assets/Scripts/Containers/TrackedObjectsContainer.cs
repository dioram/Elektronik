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
    public class TrackedObjectsContainer : ITrackedContainer<SlamTrackedObject>, IContainerTree, ILookable
    {
        public TrackedObjectsContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
        }

        #region IContainer implementation

        public IEnumerator<SlamTrackedObject> GetEnumerator() => _objects.Values.Select(p => p.Item1).ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SlamTrackedObject item)
        {
            lock (_objects)
            {
                var container = CreateTrackContainer();
                _objects[item.Id] = (item, container);
                if (IsActive)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(new[] {item}));
                }
            }
        }

        public void Clear()
        {
            lock (_objects)
            {
                foreach (var tuple in _objects)
                {
                    tuple.Value.Item2.Clear();
                }

                if (IsActive)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(_objects.Keys));
                }

                lock (Children)
                {
                    _lineContainers.Clear();
                }
                _objects.Clear();
            }
        }

        public bool Contains(SlamTrackedObject item)
        {
            return _objects.Values
                    .Select(p => p.Item1)
                    .Any(o => o.Id == item.Id);
        }

        public void CopyTo(SlamTrackedObject[] array, int arrayIndex)
        {
            _objects.Values.Select(p => p.Item1).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(SlamTrackedObject item)
        {
            lock (_objects)
            {
                if (!_objects.ContainsKey(item.Id)) return false;
                _objects[item.Id].Item2.Clear();
                lock (Children)
                {
                    _lineContainers.Remove(_objects[item.Id].Item2);
                }
                _objects.Remove(item.Id);

                if (IsActive)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {item.Id}));
                }
                return true;
            }
        }

        public int Count => _objects.Count;

        public bool IsReadOnly => false;

        public int IndexOf(SlamTrackedObject item) => item.Id;

        public void Insert(int index, SlamTrackedObject item) => Add(item);

        public void RemoveAt(int index)
        {
            lock (_objects)
            {
                if (!_objects.ContainsKey(index)) return;
                _objects[index].Item2.Clear();
                lock (Children)
                {
                    _lineContainers.Remove(_objects[index].Item2);
                }
                _objects.Remove(index);
                if (IsActive)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {index}));
                }
            }
        }

        public SlamTrackedObject this[int index]
        {
            get => _objects[index].Item1;
            set
            {
                if (_objects.ContainsKey(value.Id))
                {
                    Update(value);
                }
                else
                {
                    Add(value);
                }
            }
        }

        public event Action<IContainer<SlamTrackedObject>, AddedEventArgs<SlamTrackedObject>> OnAdded;

        public event Action<IContainer<SlamTrackedObject>, UpdatedEventArgs<SlamTrackedObject>> OnUpdated;

        public event Action<IContainer<SlamTrackedObject>, RemovedEventArgs> OnRemoved;

        public void AddRange(IEnumerable<SlamTrackedObject> items)
        {
            lock (_objects)
            {
                foreach (var item in items)
                {
                    var container = CreateTrackContainer();
                    _objects[item.Id] = (item, container);
                }

                if (IsActive)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(items));
                }
            }
        }

        public void Remove(IEnumerable<SlamTrackedObject> items)
        {
            lock (_objects)
            {
                foreach (var item in items)
                {
                    if (!_objects.ContainsKey(item.Id)) continue;
                    _objects[item.Id].Item2.Clear();
                    lock (Children)
                    {
                        _lineContainers.Remove(_objects[item.Id].Item2);
                    }
                    _objects.Remove(item.Id);
                }

                if (IsActive)
                {
                    OnRemoved?.Invoke(this, new RemovedEventArgs(items.Select(i => i.Id)));
                }
            }
        }

        public void Update(SlamTrackedObject item)
        {
            PureUpdate(item);
            if (IsActive)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(new[] {item}));
            }
        }

        public void Update(IEnumerable<SlamTrackedObject> items)
        {
            foreach (var item in items)
            {
                PureUpdate(item);
            }

            if (IsActive)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(items));
            }
        }

        #endregion

        #region IContainerTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<IContainerTree> Children => _lineContainers;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                
                foreach (var child in Children)
                {
                    child.IsActive = value;
                }

                _isActive = value;
                if (_isActive) OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(this));
                else OnRemoved?.Invoke(this, new RemovedEventArgs(_objects.Keys.ToList()));
            }
        }

        public void SetRenderer(object renderer)
        {
            if (renderer is ICloudRenderer<SlamTrackedObject> trackedRenderer)
            {
                OnAdded += trackedRenderer.OnItemsAdded;
                OnUpdated += trackedRenderer.OnItemsUpdated;
                OnRemoved += trackedRenderer.OnItemsRemoved;
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(this));
                }
            }
            else if (renderer is ICloudRenderer<SlamLine> lineRenderer)
            {
                _lineRenderer = lineRenderer;
            }
        }

        #endregion

        #region ITrackedContainer implementation

        public IList<SlamLine> GetHistory(int id) => _objects[id].Item2.ToList();

        public void AddWithHistory(SlamTrackedObject item, IList<SlamLine> history)
        {
            lock (_objects)
            {
                if (_objects.ContainsKey(item.Id)) return;

                var container = CreateTrackContainer(history);
                _objects.Add(item.Id, (item, container));
                _maxId = history.Count > 0 ? history.Max(l => l.Id) : 0;
                if (IsActive)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(new[] {item}));
                }
            }
        }

        public void AddRangeWithHistory(IEnumerable<SlamTrackedObject> items, IEnumerable<IList<SlamLine>> histories)
        {
            lock (_objects)
            {
                foreach (var (i, h) in items.Zip(histories, (i, h) => (i, h)))
                {
                    if (_objects.ContainsKey(i.Id)) return;

                    var container = CreateTrackContainer(h);
                    _objects.Add(i.Id, (i, container));
                }

                _maxId = histories.SelectMany(l => l).Max(l => l.Id);
                if (IsActive)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(items));
                }
            }
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            if (_lineContainers.Count != 0 && _lineContainers[0] is ILookable lookable)
            {
                return lookable.Look(transform);
            }

            return (transform.position, transform.rotation);
        }

        #endregion

        #region Private definitions

        private ICloudRenderer<SlamLine> _lineRenderer;
        private readonly List<IContainerTree> _lineContainers = new List<IContainerTree>();

        private readonly Dictionary<int, (SlamTrackedObject, TrackContainer)> _objects =
                new Dictionary<int, (SlamTrackedObject, TrackContainer)>();

        private int _maxId = 0;
        private bool _isActive = true;

        private TrackContainer CreateTrackContainer(IList<SlamLine> history = null)
        {
            lock (Children)
            {
                var res = new TrackContainer();
                res.SetRenderer(_lineRenderer);
                _lineContainers.Add(res);
                if (history != null) res.AddRange(history);
                return res;
            }
        }

        private void PureUpdate(SlamTrackedObject item)
        {
            var container = _objects[item.Id].Item2;
            if (container.Count == 0)
            {
                container.Add(new SlamLine(_objects[item.Id].Item1.AsPoint(), item.AsPoint(), ++_maxId));
            }
            else if (container.Last().Point1.Position == item.Position)
            {
                container.Remove(container.Last());
            }
            else
            {
                container.Add(new SlamLine(container.Last().Point2, item.AsPoint(), ++_maxId));
            }

            _objects[item.Id] = (item, container);
        }

        #endregion
    }
}