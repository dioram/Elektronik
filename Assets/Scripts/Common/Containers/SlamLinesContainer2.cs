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
    public class SlamLinesContainer2 : IConnectionsContainer<SlamLine2>
    {
        private FastLinesCloud m_linesCloud;
        private SortedDictionary<int, SlamLine2> m_connections;
        private int m_maxId = 0;
        private Queue<int> m_freeIds;

        public int Count { get => m_connections.Count; }

        public SlamLine2[] this[SlamPoint pt] => this[pt.id];

        public SlamLine2[] this[int id] => m_connections.Values.Where(c => c.pt1.id == id || c.pt2.id == id).ToArray();

        public SlamLine2 this[SlamLine2 obj] 
        {
            get => this[obj.pt1.id, obj.pt2.id];
            set => this[obj.pt1.id, obj.pt2.id] = value;
        }

        public SlamLine2 this[int id1, int id2] 
        {
            get
            {
                if (!TryGet(id1, id2, out SlamLine2 conn))
                    throw new InvalidSlamContainerOperationException($"[SlamPointsContainer.Get] Container doesn't contain point with id1({id1}) and id2({id2})");
                return conn;
            }
            set
            {
                Update(value);
            }
        }

        public SlamLinesContainer2(FastLinesCloud linesCloud)
        {
            m_connections = new SortedDictionary<int, SlamLine2>();
            m_linesCloud = linesCloud;
            m_freeIds = new Queue<int>();
        }

        public void Add(SlamLine2 obj)
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
            m_linesCloud.SetLine(connectionId, obj.pt1.position, obj.pt2.position, obj.pt1.color);
        }

        public void Add(IEnumerable<SlamLine2> objects)
        {
            foreach (var obj in objects)
            {
                Add(obj);
            }
        }

        public void ChangeColor(SlamPoint obj)
        {
            IEnumerable<int> connectionIds = m_connections
                .Where(kv => kv.Value.pt1.id == obj.id || kv.Value.pt2.id == obj.id)
                .Select(kv => kv.Key);
            foreach (var connectionId in connectionIds)
            {
                m_linesCloud.SetLineColor(connectionId, obj.color); // TODO: for different ids
            }
        }

        public void ChangeColor(SlamLine2 obj)
        {
            foreach (var connectionKV in m_connections)
            {
                if (connectionKV.Value.pt1.id == obj.pt1.id && connectionKV.Value.pt2.id == obj.pt2.id ||
                    connectionKV.Value.pt2.id == obj.pt1.id && connectionKV.Value.pt1.id == obj.pt2.id)
                {
                    m_linesCloud.SetLineColor(connectionKV.Key, connectionKV.Value.pt1.color);
                    break;
                }
            }
        }

        public void Clear()
        {
            m_linesCloud.Clear();
            m_connections.Clear();
            m_freeIds.Clear();
            m_maxId = 0;
        }

        public bool Exists(int id1, int id2) => TryGet(id1, id2, out SlamLine2 _);

        public bool Exists(SlamLine2 obj) => TryGet(obj.pt1.id, obj.pt2.id, out SlamLine2 _);

        public bool Exists(int objId) => m_connections.Values.Any(c => c.pt1.id == objId || c.pt2.id == objId);

        public SlamLine2[] GetAll() => m_connections.Values.ToArray();

        public void Remove(int id)
        {
            var connectionIds2Remove = m_connections
                .Where(kv => kv.Value.pt1.id == id || kv.Value.pt2.id == id)
                .Select(kv => kv.Key);
            foreach (var connectionId2Remove in connectionIds2Remove)
            {
                m_linesCloud.SetLine(connectionId2Remove, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                m_freeIds.Enqueue(connectionId2Remove);
            }
        }

        public void Remove(int id1, int id2)
        {
            if (TryGet(id1, id2, out KeyValuePair<int, SlamLine2> kv))
            {
                m_linesCloud.SetLine(kv.Key, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                m_connections.Remove(kv.Key);
                m_freeIds.Enqueue(kv.Key);
            }
        }

        public void Remove(SlamLine2 obj) => Remove(obj.pt1.id, obj.pt2.id);

        public void Repaint() => m_linesCloud.Repaint();

        private bool TryGet(int idx1, int idx2, out KeyValuePair<int, SlamLine2> kv)
        {
            bool isFound = false;
            kv = new KeyValuePair<int, SlamLine2>();
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

        public bool TryGet(int idx1, int idx2, out SlamLine2 value)
        {
            bool isFound = TryGet(idx1, idx2, out KeyValuePair<int, SlamLine2> result);
            value = result.Value;
            return isFound;
        }

        public bool TryGet(SlamLine2 obj, out SlamLine2 current) => TryGet(obj.pt1.id, obj.pt2.id, out current);

        public void Update(IEnumerable<SlamPoint> objs)
        {
            var changes = Enumerable.Empty<Tuple<int, SlamLine2>>();
            foreach (var key in m_connections.Keys)
            {
                var connection = m_connections[key];
                bool foundMatchedPts = false;
                SlamPoint? matchedPt1 = null;
                SlamPoint? matchedPt2 = null;

                foreach (var pt in objs)
                {
                    if (pt.id == connection.pt1.id)
                    {
                        matchedPt1 = pt;
                    }
                    if (pt.id == connection.pt2.id)
                    {
                        matchedPt2 = pt;
                    }
                    foundMatchedPts = matchedPt1.HasValue && matchedPt2.HasValue;
                    if (foundMatchedPts)
                        break;
                }

                if (!foundMatchedPts)
                    continue;

                connection = new SlamLine2(matchedPt1.Value, matchedPt2.Value);

                m_linesCloud.SetLine(key, connection.pt1.position, connection.pt2.position, matchedPt1.Value.color);
                changes = changes.Append(new Tuple<int, SlamLine2>(key, connection));
            }
            var changesArray = changes.ToArray();
            foreach (var change in changesArray)
                m_connections[change.Item1] = change.Item2;
        }

        public void Update(SlamPoint obj)
        {
            var changes = Enumerable.Empty<Tuple<int, SlamLine2>>();
            foreach (var key in m_connections.Keys)
            {
                var connection = m_connections[key];
                if (connection.pt1.id == obj.id)
                    connection = new SlamLine2(obj, connection.pt2);
                else if (connection.pt2.id == obj.id)
                    connection = new SlamLine2(connection.pt1, obj);
                if (connection.pt1.id == obj.id || connection.pt2.id == obj.id)
                {
                    m_linesCloud.SetLine(key, connection.pt1.position, connection.pt2.position, obj.color);
                    changes = changes.Append(new Tuple<int, SlamLine2>(key, connection));
                }
            }
            var changesArray = changes.ToArray();
            foreach (var change in changesArray)
                m_connections[change.Item1] = change.Item2;
        }

        public void Update(SlamLine2 obj)
        {
            if (TryGet(obj.pt1.id, obj.pt2.id, out KeyValuePair<int, SlamLine2> conn))
            {
                if (obj.pt1.id == conn.Value.pt1.id && obj.pt2.id == conn.Value.pt2.id)
                {
                    m_linesCloud.SetLine(conn.Key, obj.pt1.position, obj.pt2.position, obj.pt1.color);
                }
                else
                {
                    m_linesCloud.SetLine(conn.Key, obj.pt2.position, obj.pt1.position, obj.pt1.color);
                }
                m_connections[conn.Key] = obj;
            }
        }

        public IEnumerator<SlamLine2> GetEnumerator() => m_connections.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_connections.Values.GetEnumerator();

        public void Update(IEnumerable<SlamLine2> objs)
        {
            foreach (var obj in objs)
                Update(obj);
        }

        public void Remove(IEnumerable<SlamLine2> objs)
        {
            foreach (var obj in objs)
                Remove(obj);
        }
    }
}
