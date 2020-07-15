using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;


namespace Elektronik.Common.Clouds
{
    public class FastLinesCloud : FastCloud<ILinesMeshData, LinesMeshObjectBase>, IFastLinesCloud
    {
        private Dictionary<int, List<CloudLine>> m_linesBuffer;

        private int MaxLinesCount { get => meshObjectPrefab.MaxObjectsCount; }

        public FastLinesCloud()
        {
            m_linesBuffer = new Dictionary<int, List<CloudLine>>();
        }

        public bool Exists(int lineIdx)
        {
            int meshIdx = lineIdx / MaxLinesCount;
            if (m_data.ContainsKey(meshIdx))
            {
                return m_data[meshIdx].Exists(lineIdx % MaxLinesCount);
            }
            else
            {
                return false;
            }
        }

        public CloudLine Get(int idx)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            return m_data[meshId].Get(lineId);
        }

        public void Set(CloudLine line)
        {
            CheckMesh(line.id, out int meshId, out int lineId);
            line.pt1.offset.Scale(scale * Vector3.one);
            line.pt2.offset.Scale(scale * Vector3.one);
            var meshLine = new CloudLine(lineId, line.pt1, line.pt2);
            m_data[meshId].Set(meshLine);
        }

        public void Set(int idx, Color color1, Color color2)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            m_data[meshId].Set(lineId, color1, color2);
        }

        public void Set(int idx, Vector3 position1, Vector3 position2)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            position1.Scale(scale * Vector3.one);
            position2.Scale(scale * Vector3.one);
            m_data[meshId].Set(pointId, position1, position2);
        }

        public void Set(IEnumerable<CloudLine> lines)
        {
            Debug.Assert(m_linesBuffer.Count == 0);
            foreach (var line in lines)
            {
                CheckMesh(line.id, out var meshIdx, out var lineId);
                if (!m_linesBuffer.ContainsKey(meshIdx))
                {
                    m_linesBuffer[meshIdx] = new List<CloudLine>();
                }
                m_linesBuffer[meshIdx].Add(new CloudLine(lineId, line.pt1, line.pt2));
            }
            foreach (var packet in m_linesBuffer)
            {
                m_data[packet.Key].Set(packet.Value);
            }
            m_linesBuffer.Clear();
        }
    }
}
