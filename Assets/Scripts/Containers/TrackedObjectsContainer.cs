﻿using System;
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
    public class TrackedObjectsContainer : ITrackedContainer<SlamTrackedObject>, ISourceTree, ILookable, IVisible
    {
        public TrackedObjectsContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
            ObjectLabel = DisplayName;
        }

        public string ObjectLabel;

        #region IContainer implementation

        public IEnumerator<SlamTrackedObject> GetEnumerator() =>
                _objects.Values.Select(p => p.Item1).ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(SlamTrackedObject item)
        {
            lock (_objects)
            {
                var container = CreateTrackContainer(item);
                _objects[item.Id] = (item, container);
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(new[] {item}));
            }
        }

        public void Clear()
        {
            int[] ids;
            lock (_objects)
            {
                foreach (var tuple in _objects)
                {
                    tuple.Value.Item2.Clear();
                }

                ids = _objects.Keys.ToArray();

                lock (_lineContainers)
                {
                    _lineContainers.Clear();
                }

                _objects.Clear();
            }

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        public bool Contains(SlamTrackedObject item)
        {
            lock (_objects)
            {
                return _objects.Values
                        .Select(p => p.Item1)
                        .Any(o => o.Id == item.Id);
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

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {item.Id}));
            }

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
            lock (_objects)
            {
                if (!_objects.ContainsKey(index)) return;
                _objects[index].Item2.Clear();
                lock (_lineContainers)
                {
                    _lineContainers.Remove(_objects[index].Item2);
                }

                _objects.Remove(index);
            }

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {index}));
            }
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

        public event Action<IContainer<SlamTrackedObject>, AddedEventArgs<SlamTrackedObject>> OnAdded;

        public event Action<IContainer<SlamTrackedObject>, UpdatedEventArgs<SlamTrackedObject>> OnUpdated;

        public event Action<IContainer<SlamTrackedObject>, RemovedEventArgs> OnRemoved;

        public void AddRange(IEnumerable<SlamTrackedObject> items)
        {
            var list = items.ToList();
            lock (_objects)
            {
                foreach (var item in list)
                {
                    var container = CreateTrackContainer(item);
                    _objects[item.Id] = (item, container);
                }
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(list));
            }
        }

        public void Remove(IEnumerable<SlamTrackedObject> items)
        {
            var list = items.ToList();
            lock (_objects)
            {
                foreach (var item in list)
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

            if (IsVisible)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(i => i.Id)));
            }
        }

        public void Update(SlamTrackedObject item)
        {
            lock (_objects)
            {
                PureUpdate(item);
            }

            if (IsVisible)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(new[] {item}));
            }
        }

        public void Update(IEnumerable<SlamTrackedObject> items)
        {
            var list = items.ToList();
            lock (_objects)
            {
                foreach (var item in list)
                {
                    PureUpdate(item);
                }
            }

            if (IsVisible)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(list));
            }
        }

        #endregion

        #region IContainerTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children
        {
            get
            {
                lock (_lineContainers)
                {
                    return _lineContainers;
                }
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

        public ISourceTree ContainersOnlyCopy()
        {
            var res = new TrackedObjectsContainer(DisplayName);
            AddRangeWithHistory(_objects.Values.Select(p => p.Item1), 
                                _objects.Values.Select(p => p.Item2));
            return res;
        }

        #endregion

        #region ITrackedContainer implementation

        public IList<SlamLine> GetHistory(int id)
        {
            lock (_objects)
            {
                return _objects[id].Item2.ToList();
            }
        }

        public void AddWithHistory(SlamTrackedObject item, IList<SlamLine> history)
        {
            lock (_objects)
            {
                if (_objects.ContainsKey(item.Id)) return;

                var container = CreateTrackContainer(item, history);
                _objects.Add(item.Id, (item, container));
                _maxId = history.Count > 0 ? history.Max(l => l.Id) : 0;
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(new[] {item}));
            }
        }

        public void AddRangeWithHistory(IEnumerable<SlamTrackedObject> items, IEnumerable<IList<SlamLine>> histories)
        {
            var list = items.ToList();
            var historiesList = histories.ToList();
            lock (_objects)
            {
                foreach (var (i, h) in list.Zip(historiesList, (i, h) => (i, h)))
                {
                    if (_objects.ContainsKey(i.Id)) return;

                    var container = CreateTrackContainer(i, h);
                    _objects.Add(i.Id, (i, container));
                }

                if (historiesList.Count != 0)
                {
                    _maxId = historiesList.SelectMany(l => l).Max(l => l.Id);
                }
            }

            if (IsVisible)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(list));
            }
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            lock (_objects)
            {
                if (_objects.Count == 0) return (transform.position, transform.rotation);
            }

            var pos = _objects.First().Value.Item1.Position;
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
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(this));
                    return;
                }

                int[] ids;
                lock (_objects)
                {
                    ids = _objects.Keys.ToArray();
                }

                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region Private definitions

        private ICloudRenderer<SlamLine> _lineRenderer;
        private readonly List<ISourceTree> _lineContainers = new List<ISourceTree>();

        private readonly Dictionary<int, (SlamTrackedObject, TrackContainer)> _objects =
                new Dictionary<int, (SlamTrackedObject, TrackContainer)>();

        private int _maxId = 0;
        private bool _isVisible = true;

        private TrackContainer CreateTrackContainer(SlamTrackedObject obj, IList<SlamLine> history = null)
        {
            lock (_lineContainers)
            {
                var res = new TrackContainer();
                res.SetRenderer(_lineRenderer);
                res.DisplayName = $"Track #{obj.Id}";
                _lineContainers.Add(res);
                if (history == null)
                {
                    res.Add(new SlamLine(new SlamPoint(-1, obj.Position, obj.Color),
                                         new SlamPoint(-2, obj.Position, obj.Color)));
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