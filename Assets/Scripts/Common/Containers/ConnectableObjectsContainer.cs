using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public partial class ConnectableObjectsContainer<T> : IConnectableObjectsContainer<T> where T: ICloudItem
    {
        private List<SlamLine> m_linesBuffer;
        private SparseSquareMatrix<bool> m_table;
        private ILinesContainer<SlamLine> m_lines; 
        private ICloudObjectsContainer<T> m_objects;

        public ConnectableObjectsContainer(
            ICloudObjectsContainer<T> objects,
            ILinesContainer<SlamLine> lines)
        {
            m_linesBuffer = new List<SlamLine>();
            m_table = new SparseSquareMatrix<bool>();
            m_lines = lines;
            m_objects = objects;
        }

        public IEnumerable<SlamLine> Connections { get => m_lines; }

        public int Count => m_objects.Count;

        public bool IsReadOnly => false;

        public event Action<IEnumerable<T>> ItemsAdded;
        public event Action<IEnumerable<T>> ItemsUpdated;
        public event Action<IEnumerable<int>> ItemsRemoved;
        public event Action ItemsCleared;
        public T this[T obj] { get => m_objects[obj]; set => m_objects[obj] = value; }
        public T this[int id] { get => m_objects[id]; set => m_objects[id] = value; }
        public bool Contains(T obj) => m_objects.Contains(obj);
        public bool Contains(int objId) => m_objects.Contains(objId);
        public bool TryGet(T obj, out T current) => m_objects.TryGet(obj, out current);
        public bool TryGet(int idx, out T current) => m_objects.TryGet(idx, out current);
        public bool TryGetAsPoint(int idx, out SlamPoint point) => m_objects.TryGetAsPoint(idx, out point);
        public bool TryGetAsPoint(T obj, out SlamPoint point) => m_objects.TryGetAsPoint(obj, out point);
        public IEnumerator<T> GetEnumerator() => m_objects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_objects.GetEnumerator();
        public void Add(T obj) => m_objects.Add(obj);
        public void AddRange(IEnumerable<T> objects) => m_objects.AddRange(objects);
        public void UpdateItem(T obj)
        {
            m_objects.UpdateItem(obj);
            m_objects.TryGetAsPoint(obj, out var pt1);
            foreach (var col in m_table.GetColIndices(pt1.id))
            {
                m_objects.TryGetAsPoint(col, out var pt2);
                m_lines.UpdateItem(new SlamLine(pt1, pt2));
            }
        }
        public void UpdateItems(IEnumerable<T> objs)
        {
            m_objects.UpdateItems(objs);
            Debug.Assert(m_linesBuffer.Count == 0);
            foreach(var obj in objs)
            {
                m_objects.TryGetAsPoint(obj, out var pt1);
                foreach (var col in m_table.GetColIndices(pt1.id))
                {
                    m_objects.TryGetAsPoint(col, out var pt2);
                    m_linesBuffer.Add(new SlamLine(pt1, pt2));
                }
            }
            m_lines.UpdateItems(m_linesBuffer);
            m_linesBuffer.Clear();
        }
        private void RemoveConnections(int id)
        {
            foreach (var col in m_table.GetColIndices(id))
            {
                m_lines.Remove(id, col);
                m_table.Remove(col, id);
            }
            m_table.RemoveRow(id);
        }
        public void RemoveAt(int id)
        {
            RemoveConnections(id);
            m_objects.RemoveAt(id);
        }
        public bool Remove(T obj)
        {
            int index = m_objects.IndexOf(obj);
            if (index != -1)
            {
                RemoveConnections(index);
                m_objects.Remove(obj);
                return true;
            }
            return false;
        }
        public void Remove(IEnumerable<T> objs)
        {
            Debug.Assert(m_linesBuffer.Count == 0);

            foreach (var obj in objs)
            {
                int id = m_objects.IndexOf(obj);
                if (id != -1)
                {
                    foreach (var col in m_table.GetColIndices(id))
                    {
                        m_linesBuffer.Add(
                            new SlamLine(
                                new SlamPoint(id, default, default), 
                                new SlamPoint(col, default, default)));
                        m_table.Remove(col, id);
                    }
                    m_table.RemoveRow(id);
                }
            }
            m_lines.Remove(m_linesBuffer);
            m_linesBuffer.Clear();
            m_objects.Remove(objs);
        }
        public void Clear()
        {
            m_table.Clear();
            m_lines.Clear();
            m_objects.Clear();
        }

        private bool AddConnection(T obj1, T obj2, Action<SlamLine> adding)
        {
            if (m_objects.TryGetAsPoint(obj1, out var pt1) &&
                m_objects.TryGetAsPoint(obj2, out var pt2))
            {
                if (!m_table[pt1.id, pt2.id].HasValue)
                {
                    m_table[pt1.id, pt2.id] = m_table[pt2.id, pt1.id] = true;
                    adding(new SlamLine(pt1, pt2));
                    return true;
                }
            }
            return false;
        }

        public bool AddConnection(int id1, int id2)
        {
            if (m_objects.TryGet(id1, out var obj1) && 
                m_objects.TryGet(id2, out var obj2))
            {
                return AddConnection(obj1, obj2);
            }
            return false;
        }
        public bool AddConnection(T obj1, T obj2) => AddConnection(obj1, obj2, m_lines.Add);

        private bool RemoveConnection(int id1, int id2, Action<SlamLine> removing)
        {
            if (id1 != -1 && id2 != -1)
            {
                if (m_table[id1, id2].HasValue && m_table[id1, id2].Value)
                {
                    m_table.Remove(id1, id2); 
                    m_table.Remove(id2, id1);
                    removing(new SlamLine(id1, id2));
                    return true;
                }
            }
            return false;
        }
        public bool RemoveConnection(int id1, int id2)
            => RemoveConnection(id1, id2, l => m_lines.Remove(l));
        public bool RemoveConnection(T obj1, T obj2) 
            => RemoveConnection(m_objects.IndexOf(obj1), m_objects.IndexOf(obj2));
        public void AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(m_linesBuffer.Count == 0);
            foreach (var c in connections)
                AddConnection(m_objects[c.id1], m_objects[c.id2], line => m_linesBuffer.Add(line));
            m_lines.AddRange(m_linesBuffer);
            m_linesBuffer.Clear();
        }
        public void AddConnections(IEnumerable<(T obj1, T obj2)> connections)
        {
            Debug.Assert(m_linesBuffer.Count == 0);
            foreach (var c in connections)
                AddConnection(m_objects[c.obj1], m_objects[c.obj2], line => m_linesBuffer.Add(line));
            m_lines.AddRange(m_linesBuffer);
            m_linesBuffer.Clear();
        }
        public void RemoveConnections(IEnumerable<(int id1, int id2)> connections)
        {
            Debug.Assert(m_linesBuffer.Count == 0);
            foreach (var c in connections)
                RemoveConnection(c.id1, c.id2, m_linesBuffer.Add);
            m_lines.Remove(m_linesBuffer);
            m_linesBuffer.Clear();
        }
        public void RemoveConnections(IEnumerable<(T obj1, T obj2)> connections)
        {
            Debug.Assert(m_linesBuffer.Count == 0);
            foreach (var c in connections)
                RemoveConnection(m_objects.IndexOf(c.obj1), m_objects.IndexOf(c.obj2), line => m_linesBuffer.Add(line));
            m_lines.Remove(m_linesBuffer);
            m_linesBuffer.Clear();
        }
        public IEnumerable<(int, int)> GetAllConnections(int id)
        {
            foreach (var col in m_table.GetColIndices(id))
            {
                yield return (id, col);
            }
        }
        public IEnumerable<(int, int)> GetAllConnections(T obj) 
        {
            int idx = m_objects.IndexOf(obj);
            if (idx != -1)
            {
                return GetAllConnections(idx);
            }
            return Enumerable.Empty<(int, int)>();
        }
        public int IndexOf(T item) => m_objects.IndexOf(item);
        public void Insert(int index, T item) => m_objects.Insert(index, item);
        public void CopyTo(T[] array, int arrayIndex) => m_objects.CopyTo(array, arrayIndex);
    }
}
