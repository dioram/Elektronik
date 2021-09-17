using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.Containers.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    public class TrackedObjectsContainer
            : ITrackedContainer<SlamTrackedObject>, ISourceTreeNode, ILookable, IVisible
    {
        public TrackedObjectsContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
            ObjectLabel = DisplayName;
        }

        public string ObjectLabel;

        #region IContainer

        public IEnumerator<SlamTrackedObject> GetEnumerator()
        {
            List<SlamTrackedObject> objects;
            lock (_objects)
            {
                objects = _objects.Values.Select(p => p.Item1).ToList();
            }

            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SlamTrackedObject item)
        {
            lock (_objects)
            {
                if (_objects.ContainsKey(item.Id)) return;
                var container = CreateTrackContainer(item);
                _objects[item.Id] = (item, container);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(item));
        }

        public void Clear()
        {
            SlamTrackedObject[] items;
            lock (_objects)
            {
                foreach (var tuple in _objects)
                {
                    tuple.Value.Item2.Clear();
                }

                items = _objects.Values.Select(pair => pair.Item1).ToArray();

                lock (_lineContainers)
                {
                    _lineContainers.Clear();
                }

                _objects.Clear();
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamTrackedObject>(items));
        }

        public bool Contains(SlamTrackedObject item)
        {
            return Contains(item.Id);
        }

        public bool Contains(int id)
        {
            lock (_objects)
            {
                return _objects.Values
                        .Select(p => p.Item1)
                        .Any(o => o.Id == id);
            }
        }

        public void CopyTo(SlamTrackedObject[] array, int arrayIndex)
        {
            lock (_objects)
            {
                _objects.Values.Select(p => p.Item1).ToArray().CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(SlamTrackedObject item)
        {
            lock (_objects)
            {
                if (!_objects.ContainsKey(item.Id)) return false;
                _objects[item.Id].Item2.Clear();
                lock (_lineContainers)
                {
                    _lineContainers.Remove(_objects[item.Id].Item2);
                }

                _objects.Remove(item.Id);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamTrackedObject>(item));


            return true;
        }

        public int Count
        {
            get
            {
                lock (_objects)
                {
                    return _objects.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public int IndexOf(SlamTrackedObject item) => item.Id;

        public void Insert(int index, SlamTrackedObject item) => Add(item);

        public void RemoveAt(int index)
        {
            SlamTrackedObject item;
            lock (_objects)
            {
                if (!_objects.ContainsKey(index)) return;
                item = _objects[index].Item1;
                _objects[index].Item2.Clear();
                lock (_lineContainers)
                {
                    _lineContainers.Remove(_objects[index].Item2);
                }

                _objects.Remove(index);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamTrackedObject>(item));
        }

        public SlamTrackedObject this[int index]
        {
            get
            {
                lock (_objects)
                {
                    return _objects[index].Item1;
                }
            }
            set
            {
                bool contains;
                lock (_objects)
                {
                    contains = _objects.ContainsKey(value.Id);
                }

                if (contains)
                {
                    Update(value);
                }
                else
                {
                    Add(value);
                }
            }
        }

        public event EventHandler<AddedEventArgs<SlamTrackedObject>> OnAdded;

        public event EventHandler<UpdatedEventArgs<SlamTrackedObject>> OnUpdated;

        public event EventHandler<RemovedEventArgs<SlamTrackedObject>> OnRemoved;

        public void AddRange(IList<SlamTrackedObject> items)
        {
            if (items is null) return;
            var added = new List<SlamTrackedObject>();
            lock (_objects)
            {
                foreach (var item in items)
                {
                    if (_objects.ContainsKey(item.Id)) continue;
                    var container = CreateTrackContainer(item);
                    _objects[item.Id] = (item, container);
                    added.Add(item);
                }
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(added));
        }

        public void Remove(IList<SlamTrackedObject> items)
        {
            if (items is null) return;
            lock (_objects)
            {
                foreach (var item in items)
                {
                    if (!_objects.ContainsKey(item.Id)) continue;
                    _objects[item.Id].Item2.Clear();
                    lock (_lineContainers)
                    {
                        _lineContainers.Remove(_objects[item.Id].Item2);
                    }

                    _objects.Remove(item.Id);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamTrackedObject>(items));
        }

        public IList<SlamTrackedObject> Remove(IList<int> items)
        {
            if (items is null) return new List<SlamTrackedObject>();
            var removed = new List<SlamTrackedObject>();
            lock (_objects)
            {
                foreach (var id in items.Where(_objects.ContainsKey))
                {
                    removed.Add(_objects[id].Item1);
                    _objects[id].Item2.Clear();
                    lock (_lineContainers)
                    {
                        _lineContainers.Remove(_objects[id].Item2);
                    }

                    _objects.Remove(id);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamTrackedObject>(removed));
            return removed;
        }

        public void Update(SlamTrackedObject item)
        {
            lock (_objects)
            {
                PureUpdate(item);
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(item));
        }

        public void Update(IList<SlamTrackedObject> items)
        {
            if (items is null) return;
            lock (_objects)
            {
                foreach (var item in items)
                {
                    PureUpdate(item);
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(items));
        }

        #endregion

        #region ISourceTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTreeNode> Children
        {
            get
            {
                lock (_lineContainers)
                {
                    return _lineContainers;
                }
            }
        }

        public void AddConsumer(IDataConsumer consumer)
        {
            switch (consumer)
            {
            case ICloudRenderer<SlamTrackedObject> trackedRenderer:
                OnAdded += trackedRenderer.OnItemsAdded;
                OnUpdated += trackedRenderer.OnItemsUpdated;
                OnRemoved += trackedRenderer.OnItemsRemoved;
                _trackedObjsRenderers.Add(trackedRenderer);
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(this));
                }
                break;
            case ICloudRenderer<SimpleLine> lineRenderer:
                _lineRenderers.Add(lineRenderer);
                lock (_lineContainers)
                {
                    foreach (var container in _lineContainers)
                    {
                        container.AddConsumer(lineRenderer);
                    }
                }
                break;
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            switch (consumer)
            {
            case ICloudRenderer<SlamTrackedObject> trackedRenderer:
                OnAdded -= trackedRenderer.OnItemsAdded;
                OnUpdated -= trackedRenderer.OnItemsUpdated;
                OnRemoved -= trackedRenderer.OnItemsRemoved;
                _trackedObjsRenderers.Remove(trackedRenderer);
                break;
            case ICloudRenderer<SimpleLine> lineRenderer:
                _lineRenderers.Remove(lineRenderer);
                lock (_lineContainers)
                {
                    foreach (var container in _lineContainers)
                    {
                        container.RemoveConsumer(lineRenderer);
                    }
                }
                break;
            }
        }

        public ISourceTreeNode TakeSnapshot()
        {
            var res = new TrackedObjectsContainer(DisplayName);
            lock (_objects)
            {
                res.AddRangeWithHistory(_objects.Values.Select(p => p.Item1).ToList(),
                                        _objects.Values.Select(p => (IList<SimpleLine>)p.Item2.ToList()).ToList());
            }

            return res;
        }

        #endregion

        #region ITrackedContainer implementation

        public IList<SimpleLine> GetHistory(int id)
        {
            lock (_objects)
            {
                return _objects.ContainsKey(id) ? _objects[id].Item2.ToList() : new List<SimpleLine>();
            }
        }

        public void AddWithHistory(SlamTrackedObject item, IList<SimpleLine> history)
        {
            lock (_objects)
            {
                if (_objects.ContainsKey(item.Id)) return;

                var container = CreateTrackContainer(item, history);
                _objects.Add(item.Id, (item, container));
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(item));
        }

        public void AddRangeWithHistory(IList<SlamTrackedObject> items, IList<IList<SimpleLine>> histories)
        {
            lock (_objects)
            {
                foreach (var (i, h) in items.Zip(histories, (i, h) => (i, h)))
                {
                    if (_objects.ContainsKey(i.Id)) return;

                    var container = CreateTrackContainer(i, h);
                    _objects.Add(i.Id, (i, container));
                }
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(items));
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            Vector3 pos;
            lock (_objects)
            {
                if (_objects.Count == 0) return (transform.position, transform.rotation);
                pos = _objects.First().Value.Item1.Position;
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

                foreach (var child in Children.OfType<IVisible>())
                {
                    child.IsVisible = value;
                }

                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);

                if (_isVisible)
                {
                    foreach (var renderer in _trackedObjsRenderers)
                    {
                        renderer.OnItemsAdded(this, new AddedEventArgs<SlamTrackedObject>(this));
                    }
                }
                else
                {
                    SlamTrackedObject[] items;
                    lock (_objects)
                    {
                        items = _objects.Values.Select(pair => pair.Item1).ToArray();
                    }
                    foreach (var renderer in _trackedObjsRenderers)
                    {
                        renderer.OnItemsRemoved(this, new RemovedEventArgs<SlamTrackedObject>(items));
                    }
                }
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region Private definitions

        private readonly List<ICloudRenderer<SimpleLine>> _lineRenderers = new List<ICloudRenderer<SimpleLine>>();
        private readonly List<ICloudRenderer<SlamTrackedObject>> _trackedObjsRenderers =
                new List<ICloudRenderer<SlamTrackedObject>>();

        private readonly List<ISourceTreeNode> _lineContainers = new List<ISourceTreeNode>();

        private readonly Dictionary<int, (SlamTrackedObject, TrackContainer)> _objects =
                new Dictionary<int, (SlamTrackedObject, TrackContainer)>();

        private bool _isVisible = true;

        private TrackContainer CreateTrackContainer(SlamTrackedObject obj, IList<SimpleLine> history = null)
        {
            lock (_lineContainers)
            {
                var res = new TrackContainer(this, obj) { IsVisible = IsVisible };
                foreach (var lineRenderer in _lineRenderers)
                {
                    res.AddConsumer(lineRenderer);
                }
                foreach (var renderer in _trackedObjsRenderers)
                {
                    res.AddConsumer(renderer);
                }
                res.DisplayName = $"Track #{obj.Id}";
                _lineContainers.Add(res);
                if (history == null)
                {
                    res.Add(new SimpleLine(0, obj.Position, obj.Position, obj.Color));
                }
                else
                {
                    res.AddRange(history);
                }

                return res;
            }
        }

        private void PureUpdate(SlamTrackedObject item)
        {
            if (!_objects.ContainsKey(item.Id)) return;
            var container = _objects[item.Id].Item2;
            if (container.Count > 0
                && container.Last().BeginPos == item.Position
                && container.Last().EndPos != item.Position)
            {
                container.Remove(container.Last());
            }
            else
            {
                container.AddPointToTrack(item.AsPoint());
            }

            _objects[item.Id] = (item, container);
        }

        #endregion
    }
}