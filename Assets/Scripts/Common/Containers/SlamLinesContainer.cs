using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamLinesContainer : IConnectionsContainer<SlamLine>
    {
        private FastLinesCloud m_linesCloud;
        private SortedDictionary<int, SlamLine> m_connections;
        private int m_maxId = 0;
        private Queue<int> m_freeIds;

        public int Count { get => m_connections.Count; }
        public SlamLine[] this[SlamPoint pt] => this[pt.id];
        public SlamLine[] this[int id] => m_connections.Values.Where(c => c.pt1.id == id || c.pt2.id == id).ToArray();
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
            m_linesCloud = linesCloud;
            m_freeIds = new Queue<int>();
        }
        public void Add(SlamLine obj)
        {
            int connectionId;
            if (m_freeIds.Count > 0)
            {
                connectionId = m_freeIds.Dequeue();
            }
            else
            {
                connectionId = m_maxId++;
            }
            m_connections[connectionId] = obj;
            m_linesCloud.Set(connectionId, obj.pt1.position, obj.pt2.position, obj.pt1.color, obj.pt2.color);
        }
        public void Add(IEnumerable<SlamLine> objects)
        {
            foreach (var obj in objects)
            {
                Add(obj);
            }
        }
        public void Clear()
        {
            m_linesCloud.Clear();
            m_connections.Clear();
            m_freeIds.Clear();
            m_maxId = 0;
        }
        public bool Exists(int id1, int id2) => TryGet(id1, id2, out SlamLine _);
        public bool Exists(SlamLine obj) => TryGet(obj.pt1.id, obj.pt2.id, out SlamLine _);
        public bool Exists(int objId) => m_connections.Values.Any(c => c.pt1.id == objId || c.pt2.id == objId);
        public SlamLine[] GetAll() => m_connections.Values.ToArray();
        public void Remove(int id)
        {
            var connectionIds2Remove = m_connections
                .Where(kv => kv.Value.pt1.id == id || kv.Value.pt2.id == id)
                .Select(kv => kv.Key).ToArray();
            foreach (var connectionId2Remove in connectionIds2Remove)
            {
                m_linesCloud.Set(connectionId2Remove, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                m_freeIds.Enqueue(connectionId2Remove);
                m_connections.Remove(connectionId2Remove);
            }
        }
        public void Remove(IEnumerable<int> ids)
        {
            var lines2rm = new SortedSet<int>();
            foreach (var connectionKV in m_connections)
            {
                foreach (var id in ids)
                {
                    if (connectionKV.Value.pt1.id == id || connectionKV.Value.pt2.id == id)
                    {
                        lines2rm.Add(connectionKV.Key);
                    }
                }
            }
            Remove(lines2rm.Select(i => m_connections[i]));
        }
        public void Remove(IEnumerable<SlamPoint> pts) => Remove(pts.Select(p => p.id));
        public void Remove(IEnumerable<SlamLine> objs)
        {
            foreach (var obj in objs)
                Remove(obj);
        }
        public void Remove(int id1, int id2)
        {
            if (TryGet(id1, id2, out KeyValuePair<int, SlamLine> kv))
            {
                m_freeIds.Enqueue(kv.Key);
                m_linesCloud.Set(kv.Key, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                m_connections.Remove(kv.Key);
            }
        }
        public void Remove(SlamLine obj) => Remove(obj.pt1.id, obj.pt2.id);
        private bool TryGet(int idx1, int idx2, out KeyValuePair<int, SlamLine> kv)
        {
            bool isFound = false;
            kv = new KeyValuePair<int, SlamLine>();
            foreach (var kv_ in m_connections)
            {
                if (kv_.Value.pt1.id == idx1 && kv_.Value.pt2.id == idx2 ||
                    kv_.Value.pt2.id == idx2 && kv_.Value.pt1.id == idx1)
                {
                    kv = kv_;
                    isFound = true;
                    break;
                }
            }
            return isFound;
        }
        public bool TryGet(int idx1, int idx2, out SlamLine value)
        {
            bool isFound = TryGet(idx1, idx2, out KeyValuePair<int, SlamLine> result);
            value = result.Value;
            return isFound;
        }
        public bool TryGet(SlamLine obj, out SlamLine current) => TryGet(obj.pt1.id, obj.pt2.id, out current);
        public void Update(IEnumerable<SlamPoint> objs)
        {
            var changes = new SortedDictionary<int, SlamLine>();
            foreach (var kv in m_connections)
            {
                foreach (var obj in objs)
                {
                    if (kv.Value.pt1.id == obj.id)
                    {
                        if (changes.ContainsKey(kv.Key))
                        {
                            changes[kv.Key] = new SlamLine(obj, changes[kv.Key].pt2);
                        }
                        else
                        {
                            changes[kv.Key] = new SlamLine(obj, m_connections[kv.Key].pt2);
                        }
                    }
                    if (kv.Value.pt2.id == obj.id)
                    {
                        if (changes.ContainsKey(kv.Key))
                        {
                            changes[kv.Key] = new SlamLine(changes[kv.Key].pt1, obj);
                        }
                        else
                        {
                            changes[kv.Key] = new SlamLine(m_connections[kv.Key].pt1, obj);
                        }
                    }
                }
            }
            foreach (var kv in changes)
            {
                m_connections[kv.Key] = changes[kv.Key];
                m_linesCloud.Set(kv.Key, 
                    m_connections[kv.Key].pt1.position, m_connections[kv.Key].pt2.position, 
                    m_connections[kv.Key].pt1.color, m_connections[kv.Key].pt2.color);
            }
        }
        public void Update(SlamPoint obj)
        {
            var changes = new SortedDictionary<int, SlamLine>();

            foreach (var kv in m_connections)
            {
                if (kv.Value.pt1.id == obj.id)
                {
                    if (changes.ContainsKey(kv.Key))
                    {
                        changes[kv.Key] = new SlamLine(obj, changes[kv.Key].pt2);
                    }
                    else
                    {
                        changes[kv.Key] = new SlamLine(obj, m_connections[kv.Key].pt2);
                    }
                }
                if (kv.Value.pt2.id == obj.id)
                {
                    if (changes.ContainsKey(kv.Key))
                    {
                        changes[kv.Key] = new SlamLine(changes[kv.Key].pt1, obj);
                    }
                    else
                    {
                        changes[kv.Key] = new SlamLine(m_connections[kv.Key].pt1, obj);
                    }
                }
            }

            foreach (var kv in changes)
            {
                m_connections[kv.Key] = changes[kv.Key];
                m_linesCloud.Set(kv.Key, 
                    m_connections[kv.Key].pt1.position, m_connections[kv.Key].pt2.position, 
                    m_connections[kv.Key].pt1.color, m_connections[kv.Key].pt2.color);
            }
        }
        public void Update(SlamLine obj)
        {
            if (TryGet(obj.pt1.id, obj.pt2.id, out KeyValuePair<int, SlamLine> conn))
            {
                if (obj.pt1.id == conn.Value.pt1.id && obj.pt2.id == conn.Value.pt2.id)
                {
                    m_linesCloud.Set(conn.Key, obj.pt1.position, obj.pt2.position, obj.pt1.color, obj.pt2.color);
                }
                else
                {
                    m_linesCloud.Set(conn.Key, obj.pt2.position, obj.pt1.position, obj.pt2.color, obj.pt1.color);
                }
                m_connections[conn.Key] = obj;
            }
        }
        public void Update(IEnumerable<SlamLine> objs)
        {
            foreach (var obj in objs)
                Update(obj);
        }
        public IEnumerator<SlamLine> GetEnumerator() => m_connections.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_connections.Values.GetEnumerator();
    }
}
