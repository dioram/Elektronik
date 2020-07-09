using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public partial class ConnectableObjectsContainer<T> : IConnectableObjectsContainer<T>
    {
        private SparseSquareMatrix<bool> m_table;
        private ILinesContainer<SlamLine> m_lines; 
        private ICloudObjectsContainer<T> m_objects;

        public ConnectableObjectsContainer(
            ICloudObjectsContainer<T> objects,
            ILinesContainer<SlamLine> lines)
        {
            m_table = new SparseSquareMatrix<bool>();
            m_lines = lines;
            m_objects = objects;
        }

        public IEnumerable<SlamLine> Connections { get => m_lines; }

        public int Count => m_objects.Count;

        public bool IsReadOnly => false;

        public T this[T obj] { get => m_objects[obj]; set => m_objects[obj] = value; }
        public T this[int id] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Contains(T obj) => m_objects.Contains(obj);
        public bool Contains(int objId) => m_objects.Contains(objId);
        public IList<T> GetAll() => m_objects.GetAll();
        public bool TryGet(T obj, out T current) => m_objects.TryGet(obj, out current);
        public bool TryGet(int idx, out T current) => m_objects.TryGet(idx, out current);
        public bool TryGetAsPoint(int idx, out SlamPoint point) => m_objects.TryGetAsPoint(idx, out point);
        public bool TryGetAsPoint(T obj, out SlamPoint point) => m_objects.TryGetAsPoint(obj, out point);
        public IEnumerator<T> GetEnumerator() => m_objects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_objects.GetEnumerator();
        public void Add(T obj) => m_objects.Add(obj);
        public void Add(ReadOnlyCollection<T> objects) => m_objects.Add(objects);
        private void UpdateConnection(T obj)
        {
            m_objects.TryGetAsPoint(obj, out var pt1);
            foreach (var col in m_table.GetColIndices(pt1.id))
            {
                m_objects.TryGetAsPoint(col, out var pt2);
                m_lines.Update(new SlamLine(pt1, pt2));
            }
        }
        public void Update(T obj)
        {
            m_objects.Update(obj);
            UpdateConnection(obj);
        }
        public void Update(ReadOnlyCollection<T> objs)
        {
            m_objects.Update(objs);
            for (int i = 0; i < objs.Count; ++i)
                UpdateConnection(objs[i]);
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
            if (m_objects.TryGetAsPoint(obj, out var pt))
            {
                RemoveConnections(pt.id);
                m_objects.Remove(obj);
                return true;
            }
            return false;
        }
        public void Remove(ReadOnlyCollection<T> objs)
        {
            for (int i = 0; i < objs.Count; ++i)
            {
                int id = m_objects.IndexOf(objs[i]);
                if (id != -1)
                    RemoveConnections(id);
            }
            m_objects.Remove(objs);
        }
        public void Clear()
        {
            m_table.Clear();
            m_lines.Clear();
            m_objects.Clear();
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
        public bool AddConnection(T obj1, T obj2)
        {
            if (m_objects.TryGetAsPoint(obj1, out var pt1) &&
                m_objects.TryGetAsPoint(obj2, out var pt2))
            {
                if (!m_table[pt1.id, pt2.id].HasValue || !m_table[pt1.id, pt2.id].Value)
                {
                    m_table[pt1.id, pt2.id] = m_table[pt2.id, pt1.id] = true;
                    m_lines.Add(new SlamLine(pt1, pt2));
                    return true;
                }
            }
            return false;
        }
        public bool RemoveConnection(int id1, int id2)
        {
            if (id1 != -1 && id2 != -1)
            {
                if (m_table[id1, id2].HasValue && m_table[id1, id2].Value)
                {
                    m_table.Remove(id1, id2); m_table.Remove(id2, id1);
                    m_lines.Remove(id1, id2);
                    return true;
                }
            }
            return false;
        }
        public bool RemoveConnection(T obj1, T obj2) 
            => RemoveConnection(m_objects.IndexOf(obj1), m_objects.IndexOf(obj2));
        public bool AddConnections(IEnumerable<(int id1, int id2)> connections)
        {
            bool result = true;
            foreach (var c in connections) result = AddConnection(c.id1, c.id2);
            return result;
        }
        public bool AddConnections(IEnumerable<(T obj1, T obj2)> connections)
        {
            bool result = true;
            foreach (var c in connections) result = AddConnection(c.obj1, c.obj2);
            return result;
        }
        public bool RemoveConnections(IEnumerable<(int id1, int id2)> connections)
        {
            bool result = true;
            foreach (var c in connections) result = RemoveConnection(c.id1, c.id2);
            return result;
        }
        public bool RemoveConnections(IEnumerable<(T obj1, T obj2)> connections)
        {
            bool result = true;
            foreach (var c in connections) result = RemoveConnection(c.obj1, c.obj2);
            return result;
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
