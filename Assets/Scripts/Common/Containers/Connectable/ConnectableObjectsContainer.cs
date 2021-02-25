using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class ConnectableObjectsContainer<TCloudItem> 
            : MonoBehaviour, IClearable, IConnectableObjectsContainer<TCloudItem> 
            where TCloudItem : ICloudItem
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
        public event Action<IContainer<TCloudItem>, AddedEventArgs<TCloudItem>> OnAdded;

        [Obsolete("Never raised for now.")] 
        public event Action<IContainer<TCloudItem>, UpdatedEventArgs<TCloudItem>> OnUpdated;

        [Obsolete("Never raised for now.")] 
        public event Action<IContainer<TCloudItem>, RemovedEventArgs> OnRemoved;

        public int Count => Objects.Count;

        public bool IsReadOnly => false;
        
        public bool Contains(TCloudItem obj) => Objects.Contains(obj);
        
        public IEnumerator<TCloudItem> GetEnumerator() => Objects.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => Objects.GetEnumerator();

        public int IndexOf(TCloudItem item) => Objects.IndexOf(item);
        
        public void CopyTo(TCloudItem[] array, int arrayIndex) => Objects.CopyTo(array, arrayIndex);

        public TCloudItem this[int id]
        {
            get => Objects[id];
            set => Objects[id] = value;
        }
        
        public void Add(TCloudItem obj) => Objects.Add(obj);
        
        public void Insert(int index, TCloudItem item) => Objects.Insert(index, item);
        
        public void AddRange(IEnumerable<TCloudItem> items) => Objects.AddRange(items);

        public void UpdateItem(TCloudItem item)
        {
            Objects.UpdateItem(item);
            foreach (var secondId in _table.GetColIndices(item.Id))
            {
                Connects.UpdateItem(new SlamLine(item.Id, secondId));
            }
        }

        public void UpdateItems(IEnumerable<TCloudItem> items)
        {
            Objects.UpdateItems(items);
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var pt1 in items)
            {
                foreach (var pt2Id in _table.GetColIndices(pt1.Id))
                {
                    _linesBuffer.Add(new SlamLine(pt1.AsPoint(), Objects[pt2Id].AsPoint()));
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

        public bool Remove(TCloudItem obj)
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

        public void Remove(IEnumerable<TCloudItem> items)
        {
            Debug.Assert(_linesBuffer.Count == 0);

            foreach (var obj in items)
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
            Objects.Remove(items);
        }

        public void Clear()
        {
            _table.Clear();
            Connects.Clear();
            Objects.Clear();
        }

        #endregion

        #region IConnectableObjectsContainer implementation
        
        public IEnumerable<SlamLine> Connections => Connects;

        public bool AddConnection(int id1, int id2)
        {
            if (_table[id1, id2].HasValue) return false;
            
            _table[id1, id2] = _table[id2, id1] = true;
            return true;
        }

        private bool AddConnection(int id1, int id2, Action<SlamLine> adding)
        {
            var res = AddConnection(id1, id2);
            if (res) adding(new SlamLine(Objects[id1].AsPoint(), Objects[id2].AsPoint()));
            return res;
        }

        public bool AddConnection(TCloudItem obj1, TCloudItem obj2) => AddConnection(obj1.Id, obj2.Id, Connects.Add);

        public bool RemoveConnection(int id1, int id2) => RemoveConnection(id1, id2, l => Connects.Remove(l));

        public bool RemoveConnection(TCloudItem obj1, TCloudItem obj2) => RemoveConnection(Objects.IndexOf(obj1), Objects.IndexOf(obj2));

        public void AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                AddConnection(c.id1, c.id2, line => _linesBuffer.Add(line));
            }

            Connects.AddRange(_linesBuffer);
            _linesBuffer.Clear();
        }

        public void AddConnections(IEnumerable<(TCloudItem obj1, TCloudItem obj2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                AddConnection(c.obj1.Id, c.obj2.Id, line => _linesBuffer.Add(line));
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

        public void RemoveConnections(IEnumerable<(TCloudItem obj1, TCloudItem obj2)> connections)
        {
            Debug.Assert(_linesBuffer.Count == 0);
            foreach (var c in connections)
            {
                RemoveConnection(c.obj1.Id, c.obj2.Id, line => _linesBuffer.Add(line));
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

        public IEnumerable<(int, int)> GetAllConnections(TCloudItem obj)
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
        protected IContainer<TCloudItem> Objects;

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
                    removing(new SlamLine(Objects[id1].AsPoint(), Objects[id2].AsPoint()));
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}