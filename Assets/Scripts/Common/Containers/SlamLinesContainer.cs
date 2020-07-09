using Elektronik.Common.Clouds;
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
    public class SlamLinesContainer : ILinesContainer<SlamLine>
    {
        private FastLinesCloud m_linesCloud;
        private IDictionary<int, SlamLine> m_connections;
        private IDictionary<SlamLine, int> m_connectionIndices;
        private int m_maxId = 0;
        private Queue<int> m_freeIds;
        public int Count { get => m_connections.Count; }
        public bool IsReadOnly => false;
        public SlamLine this[int index] { get => m_connections[index]; set => Update(value); }
        public SlamLine this[SlamLine obj] 
        {
            get => this[obj.pt1.id, obj.pt2.id];
            set => this[obj.pt1.id, obj.pt2.id] = value;
        }
        public SlamLine this[int id1, int id2] 
        {
            get
            {
                if (!TryGet(id1, id2, out SlamLine conn))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Get] Container doesn't contain point with id1({id1}) and id2({id2})");
                return conn;
            }
            set
            {
                Update(value);
            }
        }
        public SlamLinesContainer(FastLinesCloud linesCloud)
        {
            m_connections = new SortedDictionary<int, SlamLine>();
            m_connectionIndices = new SortedDictionary<SlamLine, int>();
            m_linesCloud = linesCloud;
            m_freeIds = new Queue<int>();
        }
        public void Add(SlamLine obj)
        {
            int connectionId = m_freeIds.Count > 0 ? m_freeIds.Dequeue() : m_maxId++;
            m_connectionIndices[obj] = connectionId;
            m_connections[connectionId] = obj;
            m_linesCloud.Set(connectionId, obj.pt1.position, obj.pt2.position, obj.pt1.color, obj.pt2.color);
        }
        public void Add(ReadOnlyCollection<SlamLine> objects)
        {
            for (int i = 0; i < objects.Count; ++i)
                Add(objects[i]);
        }
        public void Clear()
        {
            m_linesCloud.Clear();
            m_connections.Clear();
            m_connectionIndices.Clear();
            m_freeIds.Clear();
            m_maxId = 0;
        }
        public bool Exists(int id1, int id2) => TryGet(id1, id2, out SlamLine _);
        public bool Contains(SlamLine obj) => TryGet(obj.pt1.id, obj.pt2.id, out SlamLine _);
        public IList<SlamLine> GetAll() => m_connections.Values.ToList();
        public void Remove(ReadOnlyCollection<SlamLine> objs)
        {
            for (int i = 0; i < objs.Count; ++i)
                Remove(objs[i]);
        }
        public bool Remove(int id1, int id2)
        {
            if (TryGet(id1, id2, out SlamLine l))
            {
                int index = m_connectionIndices[l];
                m_connectionIndices.Remove(l);
                m_freeIds.Enqueue(index);
                m_linesCloud.Set(index, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                return true;
            }
            return false;
        }
        public bool Remove(SlamLine obj) => Remove(obj.pt1.id, obj.pt2.id);
        public SlamLine Get(int id1, int id2)
        {
            if (TryGet(id1, id2, out SlamLine line))
            {
                return line;
            }
            throw new InvalidSlamContainerOperationException($"[SlamLinesContainer.Get] Line with id[{id1}, {id2}] doesn't exist");
        }
        public bool TryGet(int idx1, int idx2, out SlamLine value)
        {
            value = default;
            if (m_connectionIndices.TryGetValue(new SlamLine(idx1, idx2), out int idx))
            {
                value = m_connections[idx];
                return true;
            }
            return false;
        }
        public bool TryGet(SlamLine obj, out SlamLine current) => TryGet(obj.pt1.id, obj.pt2.id, out current);
        public void Update(SlamLine obj)
        {
            if (m_connectionIndices.TryGetValue(obj, out int index))
            {
                SlamLine currentLine = m_connections[index];
                if (obj.pt1.id == currentLine.pt1.id && obj.pt2.id == currentLine.pt2.id)
                {
                    m_linesCloud.Set(index, obj.pt1.position, obj.pt2.position, obj.pt1.color, obj.pt2.color);
                }
                else
                {
                    m_linesCloud.Set(index, obj.pt2.position, obj.pt1.position, obj.pt2.color, obj.pt1.color);
                }
                m_connections[index] = obj;
            }
        }
        public void Update(ReadOnlyCollection<SlamLine> objs)
        {
            for (int i = 0; i < objs.Count; ++i)
                Update(objs[i]);
        }
        public IEnumerator<SlamLine> GetEnumerator() => m_connections.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_connections.Values.GetEnumerator();
        public int IndexOf(SlamLine item)
        {
            foreach (var c in m_connections)
            {
                if (c.Value.pt1.id == item.pt1.id && c.Value.pt2.id == item.pt2.id ||
                    c.Value.pt2.id == item.pt1.id && c.Value.pt1.id == item.pt2.id)
                {
                    return c.Key;
                }    
            }
            return -1;
        }
        public void Insert(int index, SlamLine item)
            => throw new NotImplementedException();
        public void RemoveAt(int index)
        {
            var line = m_connections[index];
            Remove(line);
        }
        public void CopyTo(SlamLine[] array, int arrayIndex) 
            => m_connections.Values.CopyTo(array, arrayIndex);
    }
}
