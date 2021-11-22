﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Container for objects that should be rendered with track of it's previous positions  </summary>
    public class TrackedCloudObjectsContainer
            : ITrackedCloudContainer<SlamTrackedObject>, ILookableDataSource, IVisibleDataSource
    {
        /// <summary> Constructor. </summary>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        public TrackedCloudObjectsContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
            // ObjectLabel = DisplayName;
        }

        /// <summary> Name that will be rendered on scene near object. </summary>
        public string ObjectLabel;

        #region ICloudContainer

        /// <inheritdoc />
        public IEnumerator<SlamTrackedObject> GetEnumerator()
        {
            List<SlamTrackedObject> objects;
            lock (_objects)
            {
                objects = _objects.Values.Select(p => p.Item1).ToList();
            }

            return objects.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
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

        /// <inheritdoc cref="ICloudContainer{T}.Clear" />
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

        /// <inheritdoc />
        public bool Contains(SlamTrackedObject item)
        {
            return Contains(item.Id);
        }

        /// <inheritdoc />
        public bool Contains(int id)
        {
            lock (_objects)
            {
                return _objects.Values
                        .Select(p => p.Item1)
                        .Any(o => o.Id == id);
            }
        }

        /// <inheritdoc />
        public void CopyTo(SlamTrackedObject[] array, int arrayIndex)
        {
            lock (_objects)
            {
                _objects.Values.Select(p => p.Item1).ToArray().CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public int IndexOf(SlamTrackedObject item) => item.Id;

        /// <inheritdoc />
        public void Insert(int index, SlamTrackedObject item) => Add(item);

        /// <inheritdoc />
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

        /// <inheritdoc />
        public event EventHandler<AddedEventArgs<SlamTrackedObject>> OnAdded;

        /// <inheritdoc />
        public event EventHandler<UpdatedEventArgs<SlamTrackedObject>> OnUpdated;

        /// <inheritdoc />
        public event EventHandler<RemovedEventArgs<SlamTrackedObject>> OnRemoved;

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Update(SlamTrackedObject item)
        {
            lock (_objects)
            {
                PureUpdate(item);
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(item));
        }

        /// <inheritdoc />
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

        #region IDataSource

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children
        {
            get
            {
                lock (_lineContainers)
                {
                    return _lineContainers;
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IDataSource TakeSnapshot()
        {
            var res = new TrackedCloudObjectsContainer(DisplayName);
            lock (_objects)
            {
                res.AddRangeWithHistory(_objects.Values.Select(p => p.Item1).ToList(),
                                        _objects.Values.Select(p => (IList<SimpleLine>)p.Item2.ToList()).ToList());
            }

            return res;
        }

        #endregion

        #region ITrackedCloudContainer

        /// <inheritdoc />
        public IList<SimpleLine> GetHistory(int id)
        {
            lock (_objects)
            {
                return _objects.ContainsKey(id) ? _objects[id].Item2.ToList() : new List<SimpleLine>();
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        #region ILookableDataSource

        /// <inheritdoc />
        public Pose Look(Transform transform)
        {
            Vector3 pos;
            lock (_objects)
            {
                if (_objects.Count == 0) return new Pose(transform.position, transform.rotation);
                pos = _objects.First().Value.Item1.Position;
            }

            return new Pose(pos + (transform.position - pos).normalized,
                            Quaternion.LookRotation(pos - transform.position));
        }

        #endregion

        #region IVisibleDataSource

        /// <inheritdoc />
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;

                foreach (var child in Children.OfType<IVisibleDataSource>())
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

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private

        private readonly List<ICloudRenderer<SimpleLine>> _lineRenderers = new List<ICloudRenderer<SimpleLine>>();

        private readonly List<ICloudRenderer<SlamTrackedObject>> _trackedObjsRenderers =
                new List<ICloudRenderer<SlamTrackedObject>>();

        private readonly List<IDataSource> _lineContainers = new List<IDataSource>();

        private readonly Dictionary<int, (SlamTrackedObject, TrackCloudContainer)> _objects =
                new Dictionary<int, (SlamTrackedObject, TrackCloudContainer)>();

        private bool _isVisible = true;

        private TrackCloudContainer CreateTrackContainer(SlamTrackedObject obj, IList<SimpleLine> history = null)
        {
            lock (_lineContainers)
            {
                var res = new TrackCloudContainer(this, obj) { IsVisible = IsVisible };
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
                container.AddPointToTrack(item.ToPoint());
            }

            _objects[item.Id] = (item, container);
        }

        #endregion
    }
}