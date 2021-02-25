﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class TrackedObjectsContainer : MonoBehaviour, ITrackedContainer<SlamTrackedObject>
    {
        public CloudRendererComponent<SlamLine> LineRenderer;
        public CloudRendererComponent<SlamTrackedObject> TrackedObjectsRenderer;

        #region Unity event functions

        private void Start()
        {
            OnAdded += TrackedObjectsRenderer.OnItemsAdded;
            OnUpdated += TrackedObjectsRenderer.OnItemsUpdated;
            OnRemoved += TrackedObjectsRenderer.OnItemsRemoved;
        }

        #endregion

        #region IContainer implementation

        public IEnumerator<SlamTrackedObject> GetEnumerator() => _objects.Values.Select(p => p.Item1).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(SlamTrackedObject item)
        {
            _objects[item.Id] = (item, new TrackContainer(LineRenderer));
            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(new[] {item}));
        }

        public void Clear()
        {
            foreach (var tuple in _objects)
            {
                tuple.Value.Item2.Clear();
            }
            OnRemoved?.Invoke(this, new RemovedEventArgs(_objects.Keys));
            _objects.Clear();
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
            if (!_objects.ContainsKey(item.Id)) return false;
            _objects[item.Id].Item2.Clear();
            _objects.Remove(item.Id);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new [] {item.Id}));
            return true;
        }

        public int Count => _objects.Count;
        
        [Obsolete("Useless property. Exists only as IList implementation.")]
        public bool IsReadOnly => false;

        [Obsolete("Useless method. Exists only as IList implementation.")]
        public int IndexOf(SlamTrackedObject item) => item.Id;

        [Obsolete("Useless method. Exists only as IList implementation.")]
        public void Insert(int index, SlamTrackedObject item) => Add(item);

        public void RemoveAt(int index)
        {
            if (!_objects.ContainsKey(index)) return;
            _objects[index].Item2.Clear();
            _objects.Remove(index);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new [] {index}));
        }

        public SlamTrackedObject this[int index]
        {
            get => _objects[index].Item1;
            set
            {
                if (_objects.ContainsKey(value.Id))
                {
                    UpdateItem(value);
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
            foreach (var item in items)
            {
                _objects[item.Id] = (item, new TrackContainer(LineRenderer));
            }
            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(items));
        }

        public void Remove(IEnumerable<SlamTrackedObject> items)
        {
            foreach (var item in items)
            {
                if (!_objects.ContainsKey(item.Id)) continue;
                _objects[item.Id].Item2.Clear();
                _objects.Remove(item.Id);
            }
            OnRemoved?.Invoke(this, new RemovedEventArgs(items.Select(i => i.Id)));
        }

        public void UpdateItem(SlamTrackedObject item)
        {
            PureUpdate(item);
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(new [] {item}));
        }

        public void UpdateItems(IEnumerable<SlamTrackedObject> items)
        {
            foreach (var item in items)
            {
                PureUpdate(item);
            }
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamTrackedObject>(items));
        }

        #endregion

        #region ITrackedContainer implementation

        public IList<SlamLine> GetHistory(int id) => _objects[id].Item2.ToList();

        public void AddWithHistory(SlamTrackedObject item, IList<SlamLine> history)
        {
            if (_objects.ContainsKey(item.Id)) return;

            var container = new TrackContainer(LineRenderer);
            container.AddRange(history);
            _objects.Add(item.Id, (item, container));
            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(new []{item}));
        }

        public void AddRangeWithHistory(IEnumerable<SlamTrackedObject> items, IEnumerable<IList<SlamLine>> histories)
        {
            foreach (var (i, h) in items.Zip(histories, (i, h) => (i, h)))
            {
                if (_objects.ContainsKey(i.Id)) return;

                var container = new TrackContainer(LineRenderer);
                container.AddRange(h);
                _objects.Add(i.Id, (i, container));
            }
            OnAdded?.Invoke(this, new AddedEventArgs<SlamTrackedObject>(items));
        }

        #endregion

        #region Private definitions

        private readonly Dictionary<int, (SlamTrackedObject, TrackContainer)> _objects =
                new Dictionary<int, (SlamTrackedObject, TrackContainer)>();

        private int _maxId = 0;

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
