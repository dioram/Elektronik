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

        public SlamLinesContainer(FastLinesCloud cloud)
        {
            m_lines = new SortedDictionary<int, SlamLine>();
            m_linesCloud = cloud;
        }

        public void Add(SlamLine line)
        {
            int lineId = FastLinesCloud.GetIdxFor2VertIds(line.pointId1, line.pointId2);
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color);
            m_lines.Add(lineId, line);
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
            int lineId = FastLinesCloud.GetIdxFor2VertIds(line.pointId1, line.pointId2);
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLine(lineId, line.vert1, line.vert2, line.color);
            m_lines[lineId] = line;
        }

        public void Remove(SlamLine line)
        {
            int lineId = FastLinesCloud.GetIdxFor2VertIds(line.pointId1, line.pointId2);
            Debug.AssertFormat(m_lines.ContainsKey(lineId), "Container doesn't contain line with Id {0}", lineId);
            m_linesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
            m_lines.Remove(lineId);
        }

        public void Clear()
        {
            foreach (var lineId in m_lines.Keys)
            {
                m_linesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
            }
            m_lines.Clear();
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

        public SlamLine GetLine(int vert1Id, int vert2Id)
        {
            int lineId = FastLinesCloud.GetIdxFor2VertIds(vert1Id, vert2Id);
            return GetLine(lineId);
        }

        public bool TryGetLine(SlamLine line, out SlamLine current)
        {
            current = new SlamLine();
            int lineId = FastLinesCloud.GetIdxFor2VertIds(line.pointId1, line.pointId2);
            if (m_linesCloud.LineExists(lineId))
            {
                current = m_lines[lineId];
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
