using System;
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
    /// <summary> Special container for <see cref="SlamLine"/>s </summary>
    public class SlamLinesCloudContainer : ICloudContainer<SlamLine>, ILookableDataSource, IVisibleDataSource
    {
        public SlamLinesCloudContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? "Lines" : displayName;
        }

        #region ICloudContaitner

        /// <inheritdoc />
        public event EventHandler<AddedEventArgs<SlamLine>> OnAdded;

        /// <inheritdoc />
        public event EventHandler<UpdatedEventArgs<SlamLine>> OnUpdated;

        /// <inheritdoc />
        public event EventHandler<RemovedEventArgs<SlamLine>> OnRemoved;

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (_connections)
                {
                    return _connections.Count;
                }
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public IEnumerator<SlamLine> GetEnumerator()
        {
            lock (_connections)
            {
                return _connections.Select(kv => kv.Value).GetEnumerator();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(SlamLine item)
        {
            lock (_connections)
            {
                return _connectionsIndices.ContainsKey(item.GetIds());
            }
        }

        /// <inheritdoc />
        public bool Contains(int id)
        {
            lock (_connections)
            {
                return _connections.ContainsKey(id);
            }
        }

        /// <inheritdoc />
        public void CopyTo(SlamLine[] array, int arrayIndex)
        {
            lock (_connections)
            {
                _connections.Values.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public int IndexOf(SlamLine item)
        {
            lock (_connections)
            {
                foreach (var c in _connections)
                {
                    if (c.Value.Point1.Id == item.Point1.Id && c.Value.Point2.Id == item.Point2.Id ||
                        c.Value.Point2.Id == item.Point1.Id && c.Value.Point1.Id == item.Point2.Id)
                    {
                        return c.Key;
                    }
                }

                return -1;
            }
        }

        /// <inheritdoc />
        public SlamLine this[int index]
        {
            get
            {
                lock (_connections)
                {
                    return _connections[index];
                }
            }
            set => Update(value);
        }

        /// <inheritdoc />
        public void Add(SlamLine obj)
        {
            lock (_connections)
            {
                obj.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                _connectionsIndices[obj.GetIds()] = obj.Id;
                _connections[obj.Id] = obj;
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(obj));
        }

        /// <inheritdoc />
        public void Insert(int index, SlamLine item) => Add(item);

        /// <inheritdoc />
        public void AddRange(IList<SlamLine> items)
        {
            if (items is null) return;
            var buffer = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var obj in items)
                {
                    var line = obj;
                    line.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                    if (_connectionsIndices.ContainsKey(line.GetIds())) continue;
                    _connections[line.Id] = line;
                    _connectionsIndices[line.GetIds()] = line.Id;
                    buffer.Add(line);
                }
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(buffer));
        }

        /// <inheritdoc />
        public void Update(SlamLine item)
        {
            lock (_connections)
            {
                int index = _connectionsIndices[item.GetIds()];
                item.Id = index;
                _connections[item.Id] = item;
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(item));
        }

        /// <inheritdoc />
        public void Update(IList<SlamLine> items)
        {
            if (items is null) return;
            var updatedItems = new List<SlamLine>();
            var addedItems = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var obj in items)
                {
                    var key = obj.GetIds();
                    if (!_connectionsIndices.ContainsKey(key))
                    {
                        key = (key.Id2, key.Id1);
                    }

                    if (!_connectionsIndices.ContainsKey(key))
                    {
                        addedItems.Add(obj);
                        continue;
                    }

                    var line = obj;
                    line.Id = _connectionsIndices[key];
                    _connections[line.Id] = line;
                    updatedItems.Add(line);
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(updatedItems));

            if (addedItems.Count > 0) AddRange(addedItems);
        }

        /// <inheritdoc />
        public bool Remove(SlamLine obj)
        {
            lock (_connections)
            {
                if (!_connectionsIndices.ContainsKey(obj.GetIds())) return false;
                var index = _connectionsIndices[obj.GetIds()];
                _connections.Remove(index);
                _freeIds.Enqueue(index);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamLine>(obj));

            return true;
        }

        /// <inheritdoc />
        public void Remove(IList<SlamLine> items)
        {
            if (items is null) return;
            var removed = new List<SlamLine> { Capacity = items.Count };
            lock (_connections)
            {
                foreach (var line in items)
                {
                    var key = line.GetIds();
                    int index;
                    if (_connectionsIndices.ContainsKey(key))
                    {
                        index = _connectionsIndices[key];
                        _connectionsIndices.Remove(key);
                    }
                    else if (_connectionsIndices.ContainsKey((key.Id2, key.Id1)))
                    {
                        index = _connectionsIndices[(key.Id2, key.Id1)];
                        _connectionsIndices.Remove((key.Id2, key.Id1));
                    }
                    else continue;

                    if (!_connections.ContainsKey(index)) continue;
                    _connections.Remove(index);
                    _freeIds.Enqueue(index);
                    removed.Add(new SlamLine(line.Point1, line.Point1, index));
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamLine>(removed));
        }

        /// <inheritdoc />
        public IList<SlamLine> Remove(IList<int> items)
        {
            if (items is null) return new List<SlamLine>();
            var removed = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var line in items.Where(i => _connections.ContainsKey(i)))
                {
                    removed.Add(_connections[line]);
                    var key = _connections[line].GetIds();
                    int index;
                    if (_connectionsIndices.ContainsKey(key))
                    {
                        index = _connectionsIndices[key];
                        _connectionsIndices.Remove(key);
                    }
                    else if (_connectionsIndices.ContainsKey((key.Id2, key.Id1)))
                    {
                        index = _connectionsIndices[(key.Id2, key.Id1)];
                        _connectionsIndices.Remove((key.Id2, key.Id1));
                    }
                    else continue;

                    if (!_connections.ContainsKey(index)) continue;
                    _connections.Remove(index);
                    _freeIds.Enqueue(index);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamLine>(removed));
            return removed;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            SlamLine line;
            lock (_connections)
            {
                line = _connections[index];
            }

            Remove(line);
        }

        /// <inheritdoc cref="IDataSource.Clear" />
        public void Clear()
        {
            SlamLine[] lines;
            lock (_connections)
            {
                lines = _connections.Values.ToArray();
                _connections.Clear();
                _connectionsIndices.Clear();
                _freeIds.Clear();
                _maxId = 0;
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<SlamLine>(lines));
        }

        #endregion

        #region IDataSource

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children => Enumerable.Empty<IDataSource>();

        /// <inheritdoc />
        public void AddConsumer(IDataConsumer consumer)
        {
            if (!(consumer is ICloudRenderer<SlamLine> typedRenderer)) return;
            OnAdded += typedRenderer.OnItemsAdded;
            OnUpdated += typedRenderer.OnItemsUpdated;
            OnRemoved += typedRenderer.OnItemsRemoved;
            _renderers.Add(typedRenderer);
            if (Count > 0)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
            }
        }

        /// <inheritdoc />
        public void RemoveConsumer(IDataConsumer consumer)
        {
            if (!(consumer is ICloudRenderer<SlamLine> typedRenderer)) return;
            OnAdded -= typedRenderer.OnItemsAdded;
            OnUpdated -= typedRenderer.OnItemsUpdated;
            OnRemoved -= typedRenderer.OnItemsRemoved;
            _renderers.Remove(typedRenderer);
        }

        /// <inheritdoc />
        public IDataSource TakeSnapshot()
        {
            var res = new SlamLinesCloudContainer(DisplayName);
            List<SlamLine> list;
            lock (_connections)
            {
                list = _connections.Values.ToList();
            }

            res.AddRange(list);
            return res;
        }

        #endregion

        #region ILookableDataSource

        /// <inheritdoc />
        public Pose Look(Transform transform)
        {
            lock (_connections)
            {
                if (_connectionsIndices.Count == 0) return new Pose(transform.position, transform.rotation);

                var min = Vector3.positiveInfinity;
                var max = Vector3.negativeInfinity;
                foreach (var point in _connections
                        .Values
                        .SelectMany(l => new[] {l.Point1.Position, l.Point2.Position}))
                {
                    min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                    max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
                }

                var bounds = max - min;
                var center = (max + min) / 2;

                return new Pose(center + bounds / 2 + bounds.normalized, Quaternion.LookRotation(-bounds));
            }
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
                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
                
                if (_isVisible)
                {
                    foreach (var renderer in _renderers)
                    {
                        renderer.OnItemsAdded(this, new AddedEventArgs<SlamLine>(this));
                    }
                }
                else
                {
                    SlamLine[] lines;
                    lock (_connections)
                    {
                        lines = _connections.Values.ToArray();
                    }
                    foreach (var renderer in _renderers)
                    {
                        renderer.OnItemsRemoved(this, new RemovedEventArgs<SlamLine>(lines));
                    }
                }
            }
        }

        /// <inheritdoc />
        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private

        private bool _isVisible = true;

        private readonly IDictionary<int, SlamLine> _connections = new SortedDictionary<int, SlamLine>();
        private readonly IDictionary<(int, int), int> _connectionsIndices = new SortedDictionary<(int, int), int>();
        private List<ICloudRenderer<SlamLine>> _renderers = new List<ICloudRenderer<SlamLine>>();

        private int _maxId = 0;
        private readonly Queue<int> _freeIds = new Queue<int>();

        #endregion
    }
}