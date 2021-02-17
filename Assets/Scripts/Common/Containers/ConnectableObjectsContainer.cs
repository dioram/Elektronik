using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class ConnectableObjectsContainer<T> : MonoBehaviour, IClearable, IConnectableObjectsContainer<T> where T : ICloudItem
    {
        #region Unity events

        protected virtual void Start()
        {
            if (Connects == null)
            {
                Debug.LogError($"Connects container is not set for {name}");
            }
            
            if (Objects == null)
            {
                Debug.LogError($"Objects container is not set for {name}");
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        #endregion
        
        #region IContainer implementation

        [Obsolete("Never raised for now.")] 
        public event Action<IContainer<T>, IEnumerable<T>> ItemsAdded;

        [Obsolete("Never raised for now.")] 
        public event Action<IContainer<T>, IEnumerable<T>> ItemsUpdated;

        [Obsolete("Never raised for now.")] 
        public event Action<IContainer<T>, IEnumerable<int>> ItemsRemoved;

        public int Count => Objects.Count;

        public bool IsReadOnly => false;
        
        public bool Contains(T obj) => Objects.Contains(obj);
        
        public bool TryGet(T obj, out T current) => Objects.TryGet(obj, out current);
        
        public IEnumerator<T> GetEnumerator() => Objects.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => Objects.GetEnumerator();

        public int IndexOf(T item) => Objects.IndexOf(item);
        
        public void CopyTo(T[] array, int arrayIndex) => Objects.CopyTo(array, arrayIndex);
        
        public T this[T obj]
        {
            get => Objects[obj];
            set => Objects[obj] = value;
        }

        public T this[int id]
        {
            get => Objects[id];
            set => Objects[id] = value;
        }
        
        public void Add(T obj) => Objects.Add(obj);
        
        public void Insert(int index, T item) => Objects.Insert(index, item);
        
        public void AddRange(IEnumerable<T> objects) => Objects.AddRange(objects);

        public void UpdateItem(T obj)
        {
            Objects.UpdateItem(obj);
            Objects.TryGetAsPoint(obj, out var pt1);
            foreach (var col in _table.GetColIndices(pt1.Id))
            {
                Objects.TryGetAsPoint(col, out var pt2);
                Connects.UpdateItem(new SlamLine(pt1, pt2));
            }
        }

        public void UpdateItems(IEnumerable<T> objs)
        {
            Objects.UpdateItems(objs);
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var obj in objs)
            {
                Objects.TryGetAsPoint(obj, out var pt1);
                foreach (var col in _table.GetColIndices(pt1.Id))
                {
                    Objects.TryGetAsPoint(col, out var pt2);
                    _linesBuffer.Add(new SlamLine(pt1, pt2));
                }
            }

            Connects.UpdateItems(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void RemoveAt(int id)
        {
            RemoveConnections(id);
            Objects.RemoveAt(id);
        }

        public bool Remove(T obj)
        {
            int index = Objects.IndexOf(obj);
            if (index != -1)
            {
                RemoveConnections(index);
                Objects.Remove(obj);
                return true;
            }

            return false;
        }

        public void Remove(IEnumerable<T> objs)
        {
            Debug.Assert(_linesBuffer.Count == 0);

            foreach (var obj in objs)
            {
                int id = Objects.IndexOf(obj);
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

            Connects.Remove(_linesBuffer);
            _linesBuffer.Clear();
            Objects.Remove(objs);
        }

        public void Clear()
        {
            _table.Clear();
            Connects.Clear();
            Objects.Clear();
        }

        #endregion

        #region ICloudObjectsContainer implementation
        
        public bool Contains(int objId) => Objects.Contains(objId);
        
        public bool TryGet(int idx, out T current) => Objects.TryGet(idx, out current);

        public bool TryGetAsPoint(int idx, out SlamPoint point) => Objects.TryGetAsPoint(idx, out point);
        
        public bool TryGetAsPoint(T obj, out SlamPoint point) => Objects.TryGetAsPoint(obj, out point);
        
        #endregion

        #region IConnectableObjectsContainer implementation
        
        public IEnumerable<SlamLine> Connections => Connects; 
        
        private bool AddConnection(T obj1, T obj2, Action<SlamLine> adding)
        {
            if (Objects.TryGetAsPoint(obj1, out var pt1) &&
                Objects.TryGetAsPoint(obj2, out var pt2))
            {
                if (!_table[pt1.Id, pt2.Id].HasValue)
                {
                    _table[pt1.Id, pt2.Id] = _table[pt2.Id, pt1.Id] = true;
                    adding(new SlamLine(pt1, pt2));
                    return true;
                }
            }

            return false;
        }

        public bool AddConnection(int id1, int id2)
        {
            if (Objects.TryGet(id1, out var obj1) &&
                Objects.TryGet(id2, out var obj2))
            {
                return AddConnection(obj1, obj2);
            }

            return false;
        }

        public bool AddConnection(T obj1, T obj2) => AddConnection(obj1, obj2, Connects.Add);

        public bool RemoveConnection(int id1, int id2) => RemoveConnection(id1, id2, l => Connects.Remove(l));

        public bool RemoveConnection(T obj1, T obj2) => RemoveConnection(Objects.IndexOf(obj1), Objects.IndexOf(obj2));

        public void AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                AddConnection(Objects[c.id1], Objects[c.id2], line => _linesBuffer.Add(line));
            }

            Connects.AddRange(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void AddConnections(IEnumerable<(T obj1, T obj2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                AddConnection(Objects[c.obj1], Objects[c.obj2], line => _linesBuffer.Add(line));
            }

            Connects.AddRange(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void RemoveConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                RemoveConnection(c.id1, c.id2, _linesBuffer.Add);
            }

            Connects.Remove(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void RemoveConnections(IEnumerable<(T obj1, T obj2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                RemoveConnection(Objects.IndexOf(c.obj1), Objects.IndexOf(c.obj2), line => _linesBuffer.Add(line));
            }

            Connects.Remove(_linesBuffer);
            _linesBuffer.Clear();
        }

        public IEnumerable<(int, int)> GetAllConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                yield return (id, col);
            }
        }

        public IEnumerable<(int, int)> GetAllConnections(T obj)
        {
            int idx = Objects.IndexOf(obj);
            if (idx != -1)
            {
                return GetAllConnections(idx);
            }

            return Enumerable.Empty<(int, int)>();
        }

        
        #endregion

        #region Protected definitions
        
        protected ILinesContainer<SlamLine> Connects;
        protected ICloudObjectsContainer<T> Objects;

        #endregion
        
        #region Private definitions

        private readonly List<SlamLine> _linesBuffer = new List<SlamLine>();
        private readonly SparseSquareMatrix<bool> _table = new SparseSquareMatrix<bool>();

        private void RemoveConnections(int id)
        {
            foreach (var col in _table.GetColIndices(id))
            {
                Connects.Remove(id, col);
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
                    removing(new SlamLine(id1, id2));
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}