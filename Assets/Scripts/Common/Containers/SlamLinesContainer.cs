using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamLinesContainer : IContainer<SlamLine>, IContainerTree
    {
        public SlamLinesContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? "Lines" : displayName;
        }
        
        #region IContaitner implementation

        public event Action<IContainer<SlamLine>, AddedEventArgs<SlamLine>> OnAdded;

        public event Action<IContainer<SlamLine>, UpdatedEventArgs<SlamLine>> OnUpdated;

        public event Action<IContainer<SlamLine>, RemovedEventArgs> OnRemoved;

        public int Count => _connections.Count;

        public bool IsReadOnly => false;


        public IEnumerator<SlamLine> GetEnumerator() => _connections.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(SlamLine item) => _connectionsIndices.ContainsKey(item);

        public void CopyTo(SlamLine[] array, int arrayIndex) => _connections.Values.CopyTo(array, arrayIndex);

        public int IndexOf(SlamLine item)
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
        
        [Obsolete("Don't use this getter. You can't get valid index outside the container anyway." +
                  "It exists only as implementation of IList interface")]
        public SlamLine this[int index]
        {
            get => _connections.Values.ElementAt(index);
            set => UpdateItem(value);
        }

        public void Add(SlamLine obj)
        {
            obj.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
            _connectionsIndices[obj] = obj.Id;
            _connections[obj.Id] = obj;
            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(new[] {obj}));
        }

        public void Insert(int index, SlamLine item) => Add(item);

        public void AddRange(IEnumerable<SlamLine> items)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var obj in items)
            {
                var line = obj;
                line.Id = _freeIds.Count > 0 ? _freeIds.Dequeue() : _maxId++;
                _connections[line.Id] = line;
                _connectionsIndices[line] = line.Id;
                _linesBuffer.Add(line);
            }
            OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(_linesBuffer));
            _linesBuffer.Clear();
        }

        public void UpdateItem(SlamLine item)
        {
            int index = _connectionsIndices[item];
            item.Id = index;
            _connections[item.Id] = item;
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(new[] {item}));
        }

        public void UpdateItems(IEnumerable<SlamLine> items)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var obj in items)
            {
                var line = obj;
                line.Id = _connectionsIndices[line];
                _connections[line.Id] = line;
                _linesBuffer.Add(line);
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamLine>(_linesBuffer));
            _linesBuffer.Clear();
        }

        public bool Remove(SlamLine obj)
        {
            if (!_connectionsIndices.ContainsKey(obj)) return false;
            var index = _connectionsIndices[obj];
            _connections.Remove(index);
            _freeIds.Enqueue(index);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new []{obj.Id}));
            return true;
        }

        public void Remove(IEnumerable<SlamLine> items)
        {
            List<int> ids = new List<int>();
            foreach (var l in items)
            {
                var index = _connectionsIndices[l];
                ids.Add(index);
                _connections.Remove(index);
                _freeIds.Enqueue(index);
            }

            foreach (var line in items)
            {
                _connectionsIndices.Remove(line);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        public void RemoveAt(int index)
        {
            var line = _connections[index];
            Remove(line);
        }

        public void Clear()
        {
            var ids = _connections.Keys.ToArray();
            _connections.Clear();
            _connectionsIndices.Clear();
            _freeIds.Clear();
            _maxId = 0;
            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        #endregion

        #region IContainerTree implementation

        public string DisplayName { get; }

        public IEnumerable<IContainerTree> Children => Enumerable.Empty<IContainerTree>();
        
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value && !_isActive) OnRemoved?.Invoke(this, new RemovedEventArgs(_connections.Keys));
                else if (!value && _isActive) OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
                _isActive = value;
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

        #region Private definitions

        private readonly List<SlamLine> _linesBuffer = new List<SlamLine>();
        private bool _isActive = true;

        private readonly IDictionary<int, SlamLine> _connections = new SortedDictionary<int, SlamLine>();
        private readonly IDictionary<SlamLine, int> _connectionsIndices = new SortedDictionary<SlamLine, int>();

        private int _maxId = 0;
        private readonly Queue<int> _freeIds = new Queue<int>();

        private bool TryGet(int idx1, int idx2, out int lineId)
        {
            foreach (var pair in _connections)
            {
                if (pair.Value.Equals(new SlamLine(idx1, idx2)))
                {
                    lineId = pair.Value.Id;
                    return true;
                }
            }

            lineId = -1;
            return false;
        }

        #endregion
    }
}