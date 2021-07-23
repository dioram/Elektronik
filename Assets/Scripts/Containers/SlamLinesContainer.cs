using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using UnityEngine;

namespace Elektronik.Containers
{
    public class SlamLinesContainer : IContainer<SlamLine>, ISourceTree, ILookable, IVisible, ISnapshotable
    {
        public SlamLinesContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? "Lines" : displayName;
        }

        public void UpdatePositions(IEnumerable<SlamPoint> points)
        {
            var lines = new List<(int, SlamLine)>();
            foreach (var point in points)
            {
                lock (_connections)
                {
                    foreach (var pair in _connections)
                    {
                        SlamPoint p1;
                        SlamPoint p2;
                        if (pair.Value.Point1.Id == point.Id)
                        {
                            p1 = point;
                            p2 = pair.Value.Point2;
                        }
                        else if (pair.Value.Point2.Id == point.Id)
                        {
                            p1 = pair.Value.Point1;
                            p2 = point;
                        }
                        else continue;

                        var line = new SlamLine(p1, p2);
                        _connectionsIndices.Remove(pair.Value.GetIds());
                        _connectionsIndices[line.GetIds()] = pair.Key;
                        lines.Add((pair.Key, line));
                    }
                }
            }

            // Work around for Collection was modified exception
            lock (_connections)
            {
                foreach (var pair in lines)
                {
                    _connections[pair.Item1] = pair.Item2;
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(lines.Select(l => l.Item2)));
        }

        #region IContaitner implementation

        public event EventHandler<AddedEventArgs<SlamLine>> OnAdded;

        public event EventHandler<UpdatedEventArgs<SlamLine>> OnUpdated;

        public event EventHandler<RemovedEventArgs> OnRemoved;

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

        public bool IsReadOnly => false;

        public IEnumerator<SlamLine> GetEnumerator()
        {
            lock (_connections)
            {
                return _connections.Select(kv => kv.Value).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(SlamLine item)
        {
            lock (_connections)
            {
                return _connectionsIndices.ContainsKey(item.GetIds());
            }
        }

        public bool Contains(int id)
        {
            lock (_connections)
            {
                return _connections.ContainsKey(id);
            }
        }

        public void CopyTo(SlamLine[] array, int arrayIndex)
        {
            lock (_connections)
            {
                _connections.Values.CopyTo(array, arrayIndex);
            }
        }

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

        public void Add(SlamLine obj)
        {
            lock (_connections)
            {
                obj.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                _connectionsIndices[obj.GetIds()] = obj.Id;
                _connections[obj.Id] = obj;
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {obj}));
        }

        public void Insert(int index, SlamLine item) => Add(item);

        public void AddRange(IEnumerable<SlamLine> items)
        {
            if (items is null) return;
            var buffer = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var obj in items)
                {
                    var line = obj;
                    line.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                    _connections[line.Id] = line;
                    _connectionsIndices[line.GetIds()] = line.Id;
                    buffer.Add(line);
                }
            }

            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(buffer));
        }

        public void Update(SlamLine item)
        {
            lock (_connections)
            {
                int index = _connectionsIndices[item.GetIds()];
                item.Id = index;
                _connections[item.Id] = item;
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {item}));
        }

        public void Update(IEnumerable<SlamLine> items)
        {
            if (items is null) return;
            var updatedItems = new List<SlamLine>();
            var addedItems = new List<SlamLine>();
            lock (_connections)
            {
                foreach (var obj in items)
                {
                    if (!_connectionsIndices.ContainsKey(obj.GetIds()))
                    {
                        addedItems.Add(obj);
                        continue;
                    }

                    var line = obj;
                    line.Id = _connectionsIndices[line.GetIds()];
                    _connections[line.Id] = line;
                    updatedItems.Add(line);
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(updatedItems));

            if (addedItems.Count > 0) AddRange(addedItems);
        }

        public bool Remove(SlamLine obj)
        {
            lock (_connections)
            {
                if (!_connectionsIndices.ContainsKey(obj.GetIds())) return false;
                var index = _connectionsIndices[obj.GetIds()];
                _connections.Remove(index);
                _freeIds.Enqueue(index);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(new[] {obj.Id}));

            return true;
        }

        public void Remove(IEnumerable<SlamLine> items)
        {
            if (items is null) return;
            var ids = new List<int>();
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
                    ids.Add(index);
                    _connections.Remove(index);
                    _freeIds.Enqueue(index);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        public void RemoveAt(int index)
        {
            SlamLine line;
            lock (_connections)
            {
                line = _connections[index];
            }

            Remove(line);
        }

        public void Clear()
        {
            int[] ids;
            lock (_connections)
            {
                ids = _connections.Keys.ToArray();
                _connections.Clear();
                _connectionsIndices.Clear();
                _freeIds.Clear();
                _maxId = 0;
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        #endregion

        #region ISourceTree implementation

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public void SetRenderer(ISourceRenderer renderer)
        {
            if (renderer is ICloudRenderer<SlamLine> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                OnVisibleChanged += visible =>
                {
                    if (visible) typedRenderer.OnItemsAdded(this, new AddedEventArgs<SlamLine>(this));
                    else typedRenderer.OnClear(this);
                };
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                }
            }
        }

        #endregion

        #region ILookable implementation

        public (Vector3 pos, Quaternion rot) Look(Transform transform)
        {
            lock (_connections)
            {
                if (_connectionsIndices.Count == 0) return (transform.position, transform.rotation);

                Vector3 min = Vector3.positiveInfinity;
                Vector3 max = Vector3.negativeInfinity;
                foreach (var point in _connections
                        .Values
                        .SelectMany(l => new[] {l.Point1.Position, l.Point2.Position}))
                {
                    min = new Vector3(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y), Mathf.Min(min.z, point.z));
                    max = new Vector3(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y), Mathf.Max(max.z, point.z));
                }

                var bounds = max - min;
                var center = (max + min) / 2;

                return (center + bounds / 2 + bounds.normalized, Quaternion.LookRotation(-bounds));
            }
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
            }
        }

        public event Action<bool> OnVisibleChanged;

        public bool ShowButton => true;

        #endregion

        #region ISnapshotable

        public ISnapshotable TakeSnapshot()
        {
            var res = new SlamLinesContainer(DisplayName);
            List<SlamLine> list;
            lock (_connections)
            {
                list = _connections.Values.ToList();
            }

            res.AddRange(list);
            return res;
        }

        public void WriteSnapshot(IDataRecorderPlugin recorder)
        {
            recorder.OnAdded(DisplayName, this.ToList());
        }

        #endregion

        #region Private definitions

        private bool _isVisible = true;

        private readonly IDictionary<int, SlamLine> _connections = new SortedDictionary<int, SlamLine>();
        private readonly IDictionary<(int, int), int> _connectionsIndices = new SortedDictionary<(int, int), int>();

        private int _maxId = 0;
        private readonly Queue<int> _freeIds = new Queue<int>();

        #endregion
    }
}