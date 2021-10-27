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
    /// <summary> Container for  </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    public class ConnectableObjectsCloudContainer<TCloudItem> : IConnectableObjectsCloudContainer<TCloudItem>,
                                                                ILookableDataSource, IVisibleDataSource
            where TCloudItem : struct, ICloudItem
    {
        public ConnectableObjectsCloudContainer(ICloudContainer<TCloudItem> objects,
                                                ICloudContainer<SlamLine> connects,
                                                string displayName = "",
                                                SparseSquareMatrix<bool> table = null)
        {
            _connects = connects;
            _objects = objects;
            _table = table ?? new SparseSquareMatrix<bool>();
            Children = new IDataSource[] { _connects, _objects, };

            DisplayName = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;
        }

        #region IContainer

        public event EventHandler<AddedEventArgs<TCloudItem>> OnAdded;

        public event EventHandler<UpdatedEventArgs<TCloudItem>> OnUpdated;

        public event EventHandler<RemovedEventArgs<TCloudItem>> OnRemoved;

        public int Count => _objects.Count;

        public bool IsReadOnly => false;


        public bool Contains(int id) => _objects.Contains(id);

        public bool Contains(TCloudItem obj) => Contains(obj.Id);

        public IEnumerator<TCloudItem> GetEnumerator() => _objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _objects.GetEnumerator();

        public int IndexOf(TCloudItem item) => _objects.IndexOf(item);

        public void CopyTo(TCloudItem[] array, int arrayIndex) => _objects.CopyTo(array, arrayIndex);

        public TCloudItem this[int id]
        {
            get => _objects[id];
            set => _objects[id] = value;
        }

        public void Add(TCloudItem obj)
        {
            lock (_table)
            {
                _objects.Add(obj);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(obj));
        }

        public void Insert(int index, TCloudItem item)
        {
            lock (_table)
            {
                _objects.Insert(index, item);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(item));
        }

        public void AddRange(IList<TCloudItem> items)
        {
            if (items is null) return;
            lock (_table)
            {
                _objects.AddRange(items);
            }

            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(items));
        }

        public void Update(TCloudItem item)
        {
            lock (_table)
            {
                _objects.Update(item);
                foreach (var secondId in _table.GetColIndices(item.Id))
                {
                    _connects.Update(new SlamLine(item.Id, secondId));
                }
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(item));
        }

        public void Update(IList<TCloudItem> items)
        {
            if (items is null) return;
            if (items.Count == 0) return;
            lock (_table)
            {
                _objects.Update(items);
                _linesBuffer.Clear();
                foreach (var pt1 in items)
                {
                    foreach (var pt2Id in _table.GetColIndices(pt1.Id))
                    {
                        _linesBuffer.Add(new SlamLine(pt1.ToPoint(), _objects[pt2Id].ToPoint()));
                    }
                }

                if (_linesBuffer.Count > 0) _connects.Update(_linesBuffer);
            }

            OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(items));
        }

        public void RemoveAt(int id)
        {
            TCloudItem item;
            lock (_table)
            {
                RemoveConnections(id);
                item = _objects[id];
                _objects.RemoveAt(id);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(item));
        }

        public bool Remove(TCloudItem obj)
        {
            lock (_table)
            {
                var index = _objects.IndexOf(obj);
                if (index == -1) return false;

                RemoveConnections(index);
                _objects.Remove(obj);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(obj));
            return true;
        }

        public void Remove(IList<TCloudItem> items)
        {
            if (items is null) return;
            _linesBuffer.Clear();
            lock (_table)
            {
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
                _objects.Remove(items);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(items));
        }

        public IList<TCloudItem> Remove(IList<int> items)
        {
            if (items is null) return new List<TCloudItem>();
            _linesBuffer.Clear();
            IList<TCloudItem> removed;
            lock (_table)
            {
                foreach (var id in items)
                {
                    if (id == -1) continue;

                    foreach (var col in _table.GetColIndices(id))
                    {
                        _linesBuffer.Add(new SlamLine(new SlamPoint(id, default, default),
                                                      new SlamPoint(col, default, default)));
                        _table.Remove(col, id);
                    }

                    _table.RemoveRow(id);
                }

                _connects.Remove(_linesBuffer);
                removed = _objects.Remove(items);
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(removed));
            return removed;
        }

        public void Clear()
        {
            var list = _objects.ToArray();
            lock (_table)
            {
                _table.Clear();
                _connects.Clear();
                _objects.Clear();
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(list));
        }

        #endregion

        #region IConnectableObjectsContainer implementation

        public void AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            if (connections is null) return;
            _linesBuffer.Clear();
            var list = connections.ToList();
            foreach (var c in list)
            {
                AddConnection(c.id1, c.id2, line => _linesBuffer.Add(line));
            }

            _connects.AddRange(_linesBuffer);
            OnConnectionsUpdated?.Invoke(this, new ConnectionsEventArgs(list));
        }

        public void RemoveConnections(IEnumerable<(int id1, int id2)> connections)
        {
            if (connections is null) return;
            _linesBuffer.Clear();
            var list = connections.ToList();
            foreach (var c in list)
            {
                RemoveConnection(c.id1, c.id2, _linesBuffer.Add);
            }

            _connects.Remove(_linesBuffer);
            OnConnectionsRemoved?.Invoke(this, new ConnectionsEventArgs(list));
        }

        public IEnumerable<int> GetConnections(int id)
        {
            return _table.GetColIndices(id);
        }

        public IEnumerable<int> GetConnections(TCloudItem obj)
        {
            var idx = _objects.IndexOf(obj);
            return idx != -1 ? GetConnections(idx) : Enumerable.Empty<int>();
        }

        public event EventHandler<ConnectionsEventArgs> OnConnectionsUpdated;

        public event EventHandler<ConnectionsEventArgs> OnConnectionsRemoved;

        #endregion

        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<IDataSource> Children { get; }

        public void AddConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.AddConsumer(consumer);
            }
        }

        public void RemoveConsumer(IDataConsumer consumer)
        {
            foreach (var child in Children)
            {
                child.RemoveConsumer(consumer);
            }
        }

        public IDataSource TakeSnapshot()
        {
            var objects = _objects.TakeSnapshot() as ICloudContainer<TCloudItem>;
            var connects = _connects.TakeSnapshot() as ICloudContainer<SlamLine>;
            return new ConnectableObjectsCloudContainer<TCloudItem>(objects, connects, DisplayName, _table.DeepCopy());
        }

        #endregion

        #region ILookable

        public Pose Look(Transform transform)
        {
            return (_objects as ILookableDataSource)?.Look(transform) ?? new Pose(transform.position, transform.rotation);
        }

        #endregion

        #region IVisible

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                foreach (var child in Children.OfType<IVisibleDataSource>())
                {
                    child.IsVisible = value;
                }

                _isVisible = value;
                OnVisibleChanged?.Invoke(_isVisible);
            }
        }

        public event Action<bool> OnVisibleChanged;

        #endregion

        #region Private definitions

        private readonly List<SlamLine> _linesBuffer = new List<SlamLine>();
        private readonly SparseSquareMatrix<bool> _table;
        private readonly ICloudContainer<SlamLine> _connects;
        private readonly ICloudContainer<TCloudItem> _objects;
        private bool _isVisible = true;

        public IEnumerable<SlamLine> Connections => _connects;

        private bool AddConnection(int id1, int id2)
        {
            if (_table[id1, id2].HasValue) return false;

            _table[id1, id2] = _table[id2, id1] = true;
            return true;
        }

        private bool AddConnection(int id1, int id2, Action<SlamLine> adding)
        {
            var res = AddConnection(id1, id2);
            if (res && _objects.Contains(new TCloudItem { Id = id1 }) && _objects.Contains(new TCloudItem { Id = id2 }))
            {
                adding(new SlamLine(_objects[id1].ToPoint(), _objects[id2].ToPoint()));
            }

            return res;
        }

        private void RemoveConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                _connects.Remove(new SlamLine(id, col));
                _table.Remove(col, id);
            }

            _table.RemoveRow(id);
        }

        private bool RemoveConnection(int id1, int id2, Action<SlamLine> removing)
        {
            if (id1 != -1 && id2 != -1)
            {
                if (_table.Contains(id1, id2) && _table[id1, id2].HasValue && _table[id1, id2].Value)
                {
                    _table.Remove(id1, id2);
                    _table.Remove(id2, id1);
                    removing(new SlamLine(_objects[id1].ToPoint(), _objects[id2].ToPoint()));
                    return true;
                }

                Debug.LogWarning($"Connection {id1} - {id2} not exists.");
            }

            return false;
        }

        #endregion
    }
}