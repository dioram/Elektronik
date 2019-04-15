using Elektronik.Common.Clouds;
using Elektronik.Common.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamLinesContainer : ISlamLinesContainer<SlamLine>
    {
        private readonly FastLinesCloud m_linesCloud;
        private readonly SortedDictionary<int, SlamLine> m_lines;
        private readonly SortedDictionary<long, int> m_longId2Id;
        private readonly Queue<int> m_indices;

        private int m_added = 0;
        private int m_removed = 0;
        private int m_diff = 0;

        public SlamLine this[SlamLine obj]
        {
            get => this[m_longId2Id[obj.GenerateLongId()]];
            set => this[m_longId2Id[obj.GenerateLongId()]] = value;
        }
        public SlamLine this[int id]
        {
            get
            {
                Debug.AssertFormat(
                    m_lines.ContainsKey(id),
                    "[SlamLinesContainer.Get] Container doesn't contain line with id {0}", id);
                return m_lines[id];
            }
            set
            {
                if (TryGet(id, out _)) Update(value); else Add(value);
            }
        }

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
            Debug.AssertFormat(
                m_lines.ContainsKey(lineId), 
                "[SlamLinesContainer.Remove] Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
            m_lines.Remove(lineId);
            m_longId2Id.Remove(longId);
            m_indices.Enqueue(lineId);
        }

        public void Remove(int lineId) => Remove(m_lines[lineId]);

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
            Debug.AssertFormat(
                m_lines.ContainsKey(lineId), 
                "[SlamLinesContainer.Update] Container doesn't contain line with Id {0}", lineId);
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
            Debug.LogFormat("[SlamLinesContainer.Clear] Added lines : {0}; Removed lines: {1}; Diff: {2}", m_added, m_removed, m_diff);
            m_added = 0;
            m_removed = 0;
        }

        public SlamLine[] GetAll() => m_lines.Select(kv => kv.Value).ToArray();

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
        public bool TryGet(int idx, out SlamLine current)
        {
            current = new SlamLine();
            if (m_longId2Id.TryGetValue(idx, out int lineId))
            {
                current = m_lines[lineId];
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGet(SlamLine line, out SlamLine lineFromContainer) => TryGet(m_longId2Id[line.GenerateLongId()], out lineFromContainer);

        public void ChangeColor(SlamLine line)
        {
            long longId = line.GenerateLongId();
            int lineId = m_longId2Id[longId];
            Debug.AssertFormat(
                m_lines.ContainsKey(lineId), 
                "[SlamLinesContainer.ChangeColor] Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLineColor(lineId, line.color1);
            SlamLine currentLine = m_lines[lineId];
            currentLine.color1 = line.color1;
            m_lines[lineId] = currentLine;
        }

        public bool Exists(SlamLine line)
        {
            long longId = line.GenerateLongId();
            return m_longId2Id.TryGetValue(longId, out _);
        }

        public void Repaint() => m_linesCloud.Repaint();

        public bool Exists(int id1, int id2)=> m_longId2Id.TryGetValue(SlamLine.GenerateLongId(id1, id2), out _);

        public bool Exists(int objId) => m_lines.ContainsKey(objId);

        public void Remove(int id1, int id2) => Remove(Get(id1, id2));

        public IEnumerator<SlamLine> GetEnumerator() => m_lines.Select(kv => kv.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_lines.Select(kv => kv.Value).GetEnumerator();        
    }
}
