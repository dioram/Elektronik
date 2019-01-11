using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamLinesContainer : ISlamContainer<SlamLine>
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
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color);
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
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color);
            m_lines[lineId] = line;
        }

        public void Clear()
        {
            SlamLine[] lines = GetAll();
            for (int i = 0; i < lines.Length; ++i)
            {
                Remove(lines[i]);
            }
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

        public bool TryGet(SlamLine line, out SlamLine current)
        {
            current = new SlamLine();
            int lineId = -1;
            if (m_longId2Id.TryGetValue(line.GenerateLongId(), out lineId))
            {
                current = m_lines[lineId];
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
            m_linesCloud.SetLineColor(lineId, line.color);
            SlamLine currentLine = m_lines[lineId];
            currentLine.color = line.color;
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
    }
}
