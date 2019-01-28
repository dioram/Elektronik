using Elektronik.Common.Clouds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamLinesContainer : ISlamLinesContainer<SlamLine>
    {
        private FastLinesCloud m_linesCloud;
        private SortedDictionary<int, SlamLine> m_lines;
        private SortedDictionary<long, int> m_longId2Id;
        private Queue<int> m_indices;

        private int m_added = 0;
        private int m_removed = 0;
        private int m_diff = 0;

        public SlamLinesContainer(FastLinesCloud cloud)
        {
            m_longId2Id = new SortedDictionary<long, int>();
            m_indices = new Queue<int>(Enumerable.Range(0, 100000));
            m_lines = new SortedDictionary<int, SlamLine>();
            m_linesCloud = cloud;
        }

        public int Add(SlamLine line)
        {
            ++m_diff;
            ++m_added;
            long longId = line.GenerateLongId();
            int lineId = m_indices.Dequeue();
            m_longId2Id.Add(longId, lineId);
            m_lines.Add(lineId, line);
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color1);
            return lineId;
        }

        public void Remove(SlamLine line)
        {
            --m_diff;
            ++m_removed;
            long longId = line.GenerateLongId();
            int lineId = m_longId2Id[longId];
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
            m_lines.Remove(lineId);
            m_longId2Id.Remove(longId);
            m_indices.Enqueue(lineId);
        }

        public void Remove(int lineId)
        {
            Remove(m_lines[lineId]);
        }

        public void AddRange(SlamLine[] lines)
        {
            foreach (var line in lines)
            {
                Add(line);
            }
        }

        public void Update(SlamLine line)
        {
            long longId = line.GenerateLongId();
            int lineId = m_longId2Id[longId];
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color1);
            m_lines[lineId] = line;
        }

        public void Clear()
        {
            SlamLine[] lines = GetAll();
            for (int i = 0; i < lines.Length; ++i)
            {
                Remove(lines[i]);
            }
            m_linesCloud.Clear();
            Repaint();
            Debug.LogFormat("[Clear] Added lines : {0}; Removed lines: {1}; Diff: {2}", m_added, m_removed, m_diff);
            m_added = 0;
            m_removed = 0;
        }

        public SlamLine[] GetAll()
        {
            return m_lines.Select(kv => kv.Value).ToArray();
        }

        public void Set(SlamLine line)
        {
            SlamLine buttPlug;
            if (TryGet(line, out buttPlug))
            {
                Update(line);
            }
            else
            {
                Add(line);
            }
        }

        public SlamLine Get(int lineId)
        {
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with id {0}", lineId);
            return m_lines[lineId];
        }

        public SlamLine Get(SlamLine line)
        {
            int lineId = m_longId2Id[line.GetHashCode()];
            return Get(lineId);
        }

        public SlamLine Get(int id1, int id2)
        {
            return m_lines
                .Where(kv => 
                    kv.Value.pointId1 == id1 && kv.Value.pointId2 == id2 ||
                    kv.Value.pointId2 == id1 && kv.Value.pointId1 == id2)
                .Select(kv => kv.Value).First();
        }

        public bool TryGet(int id1, int id2, out SlamLine line)
        {
            line = new SlamLine();
            if (m_lines.Any(kv => 
                kv.Value.pointId1 == id1 && kv.Value.pointId2 == id2 ||
                kv.Value.pointId2 == id1 && kv.Value.pointId1 == id2))
            {
                line = Get(id1, id2);
                return true;
            }
            return false;
        }

        public bool TryGet(SlamLine line, out SlamLine lineFromContainer)
        {
            lineFromContainer = new SlamLine();
            int lineId = -1;
            if (m_longId2Id.TryGetValue(line.GenerateLongId(), out lineId))
            {
                lineFromContainer = m_lines[lineId];
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeColor(SlamLine line)
        {
            long longId = line.GenerateLongId();
            int lineId = m_longId2Id[longId];
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLineColor(lineId, line.color1);
            SlamLine currentLine = m_lines[lineId];
            currentLine.color1 = line.color1;
            m_lines[lineId] = currentLine;
        }

        public bool Exists(SlamLine line)
        {
            long longId = line.GenerateLongId();
            int lineId;
            return m_longId2Id.TryGetValue(longId, out lineId);
        }

        public void Repaint()
        {
            m_linesCloud.Repaint();
        }

        public bool Exists(int id1, int id2)
        {
            int lineId;
            return m_longId2Id.TryGetValue(SlamLine.GenerateLongId(id1, id2), out lineId);
        }

        public bool Exists(int objId)
        {
            return m_lines.ContainsKey(objId);
        }

        public void Remove(int id1, int id2)
        {
            Remove(Get(id1, id2));
        }
    }
}
