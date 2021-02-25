using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers.NotMono
{
    public class ConnectableObjectsContainer<TCloudItem> : IClearable, IConnectableObjectsContainer<TCloudItem>,
                                                           IContainerTree
            where TCloudItem : struct, ICloudItem
    {
        public ConnectableObjectsContainer(IContainer<TCloudItem> objects,
                                           ILinesContainer<SlamLine> connects,
                                           string displayName = "")
        {
            _connects = connects;
            _objects = objects;
            Children = new[]
            {
                    (IContainerTree) _connects,
                    (IContainerTree) _objects,
            };

            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
        }

        #region IContainer implementation

        [Obsolete("Never raised for now.")]
        public event Action<IContainer<TCloudItem>, AddedEventArgs<TCloudItem>> OnAdded;

        [Obsolete("Never raised for now.")]
        public event Action<IContainer<TCloudItem>, UpdatedEventArgs<TCloudItem>> OnUpdated;

        [Obsolete("Never raised for now.")] public event Action<IContainer<TCloudItem>, RemovedEventArgs> OnRemoved;

        public int Count => _objects.Count;

        public bool IsReadOnly => false;

        public bool Contains(TCloudItem obj) => _objects.Contains(obj);

        public IEnumerator<TCloudItem> GetEnumerator() => _objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _objects.GetEnumerator();

        public int IndexOf(TCloudItem item) => _objects.IndexOf(item);

        public void CopyTo(TCloudItem[] array, int arrayIndex) => _objects.CopyTo(array, arrayIndex);

        public TCloudItem this[int id]
        {
            get => _objects[id];
            set => _objects[id] = value;
        }

        public void Add(TCloudItem obj) => _objects.Add(obj);

        public void Insert(int index, TCloudItem item) => _objects.Insert(index, item);

        public void AddRange(IEnumerable<TCloudItem> items) => _objects.AddRange(items);

        public void UpdateItem(TCloudItem item)
        {
            _objects.UpdateItem(item);
            foreach (var secondId in _table.GetColIndices(item.Id))
            {
                _connects.UpdateItem(new SlamLine(item.Id, secondId));
            }
        }

        public void UpdateItems(IEnumerable<TCloudItem> items)
        {
            _objects.UpdateItems(items);
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var pt1 in items)
            {
                foreach (var pt2Id in _table.GetColIndices(pt1.Id))
                {
                    _linesBuffer.Add(new SlamLine(pt1.AsPoint(), _objects[pt2Id].AsPoint()));
                }
            }

            _connects.UpdateItems(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void RemoveAt(int id)
        {
            RemoveConnections(id);
            _objects.RemoveAt(id);
        }

        public bool Remove(TCloudItem obj)
        {
            int index = _objects.IndexOf(obj);
            if (index != -1)
            {
                RemoveConnections(index);
                _objects.Remove(obj);
                return true;
            }

            return false;
        }

        public void Remove(IEnumerable<TCloudItem> items)
        {
            Debug.Assert(_linesBuffer.Count == 0);

            foreach (var obj in items)
            {
                int id = _objects.IndexOf(obj);
                if (id != -1)
                {
                    foreach (var col in _table.GetColIndices(id))
                    {
                        _linesBuffer.Add(
                                new SlamLine(
                                        new SlamPoint(id, default, default),
                                        new SlamPoint(col, default, default)));
                        _table.Remove(col, id);
                    }

                    _table.RemoveRow(id);
                }
            }

            _connects.Remove(_linesBuffer);
            _linesBuffer.Clear();
            _objects.Remove(items);
        }

        public void Clear()
        {
            _table.Clear();
            _connects.Clear();
            _objects.Clear();
        }

        #endregion

        #region IConnectableObjectsContainer implementation

        public IEnumerable<SlamLine> Connections => _connects;

        public bool AddConnection(int id1, int id2)
        {
            if (_table[id1, id2].HasValue) return false;

            _table[id1, id2] = _table[id2, id1] = true;
            return true;
        }

        private bool AddConnection(int id1, int id2, Action<SlamLine> adding)
        {
            var res = AddConnection(id1, id2);
            if (res) adding(new SlamLine(_objects[id1].AsPoint(), _objects[id2].AsPoint()));
            return res;
        }

        public bool AddConnection(TCloudItem obj1, TCloudItem obj2) => AddConnection(obj1.Id, obj2.Id, _connects.Add);

        public bool RemoveConnection(int id1, int id2) => RemoveConnection(id1, id2, l => _connects.Remove(l));

        public bool RemoveConnection(TCloudItem obj1, TCloudItem obj2) =>
                RemoveConnection(_objects.IndexOf(obj1), _objects.IndexOf(obj2));

        public void AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                AddConnection(c.id1, c.id2, line => _linesBuffer.Add(line));
            }

            _connects.AddRange(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void AddConnections(IEnumerable<(TCloudItem obj1, TCloudItem obj2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                AddConnection(c.obj1.Id, c.obj2.Id, line => _linesBuffer.Add(line));
            }

            _connects.AddRange(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void RemoveConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                RemoveConnection(c.id1, c.id2, _linesBuffer.Add);
            }

            _connects.Remove(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void RemoveConnections(IEnumerable<(TCloudItem obj1, TCloudItem obj2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                RemoveConnection(c.obj1.Id, c.obj2.Id, line => _linesBuffer.Add(line));
            }

            _connects.Remove(_linesBuffer);
            _linesBuffer.Clear();
        }

        public IEnumerable<(int, int)> GetAllConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                yield return (id, col);
            }
        }

        public IEnumerable<(int, int)> GetAllConnections(TCloudItem obj)
        {
            int idx = _objects.IndexOf(obj);
            if (idx != -1)
            {
                return GetAllConnections(idx);
            }

            return Enumerable.Empty<(int, int)>();
        }

        #endregion

        #region IContainerTree imlementation

        public string DisplayName { get; protected set; }

        public IEnumerable<IContainerTree> Children { get; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                foreach (var child in Children)
                {
                    child.IsActive = value;
                }

                _isActive = value;
            }
        }

        public void SetRenderer(object renderer)
        {
            foreach (var child in Children)
            {
                child.SetRenderer(renderer);
            }
        }

        #endregion

        #region Private definitions

        private readonly List<SlamLine> _linesBuffer = new List<SlamLine>();
        private readonly SparseSquareMatrix<bool> _table = new SparseSquareMatrix<bool>();
        private readonly ILinesContainer<SlamLine> _connects;
        private readonly IContainer<TCloudItem> _objects;
        private bool _isActive = true;

        private void RemoveConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                _connects.Remove(id, col);
                _table.Remove(col, id);
            }

            _table.RemoveRow(id);
        }

        private bool RemoveConnection(int id1, int id2, Action<SlamLine> removing)
        {
            if (id1 != -1 && id2 != -1)
            {
                if (_table[id1, id2].HasValue && _table[id1, id2].Value)
                {
                    _table.Remove(id1, id2);
                    _table.Remove(id2, id1);
                    removing(new SlamLine(_objects[id1].AsPoint(), _objects[id2].AsPoint()));
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}