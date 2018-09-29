using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public class SlamLinesContainer
    {
        private FastLinesCloud m_linesCloud;
        private SortedDictionary<int, SlamLine> m_lines;
        private SortedDictionary<long, int> m_longId2Id;
        private Queue<int> m_indices;

        public SlamLinesContainer(FastLinesCloud cloud)
        {
            m_longId2Id = new SortedDictionary<long, int>();
            m_indices = new Queue<int>(Enumerable.Range(0, 100000));
            m_lines = new SortedDictionary<int, SlamLine>();
            m_linesCloud = cloud;
        }

        public int Add(SlamLine line)
        {
            long longId = line.GenerateLongId();
            int lineId = m_indices.Dequeue();
            m_longId2Id.Add(longId, lineId);
            m_lines.Add(lineId, line);
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color);
            return lineId;
        }

        public void Remove(SlamLine line)
        {
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
            int lineId = m_longId2Id[line.GetHashCode()];
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color);
            m_lines[lineId] = line;
        }

        

        public void Clear()
        {
            SlamLine[] lines = GetAllSlamLines();
            for (int i = 0; i < lines.Length; ++i)
            {
                Remove(lines[i]);
            }
            Repaint();
        }

        public SlamLine[] GetAllSlamLines()
        {
            return m_lines.Select(kv => kv.Value).ToArray();
        }

        public void SetLine(SlamLine line)
        {
            SlamLine buttPlug;
            if (TryGetLine(line, out buttPlug))
            {
                Update(line);
            }
            else
            {
                Add(line);
            }
        }

        public SlamLine GetLine(int lineId)
        {
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with id {0}", lineId);
            return m_lines[lineId];
        }

        public SlamLine GetLine(SlamLine line)
        {
            int lineId = m_longId2Id[line.GetHashCode()];
            return GetLine(lineId);
        }

        public bool TryGetLine(SlamLine line, out SlamLine current)
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

        public bool LineExists(SlamLine line)
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
