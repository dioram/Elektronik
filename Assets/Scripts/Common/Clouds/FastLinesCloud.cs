using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;


namespace Elektronik.Common.Clouds
{
    public class FastLinesCloud : FastCloud<ILinesMeshData, LinesMeshObjectBase>, IFastLinesCloud
    {
        private int MaxLinesCount { get => meshObjectPrefab.MaxObjectsCount; }

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

        public void Get(int idx, out Vector3 position1, out Vector3 position2, out Color color)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            m_data[meshId].Get(lineId, out position1, out position2, out color);
        }

        public void Set(int idx, Vector3 position1, Vector3 position2, Color color1, Color color2)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            position1.Scale(scale * Vector3.one);
            position2.Scale(scale * Vector3.one);
            m_data[meshId].Set(lineId, position1, position2, color1, color2);
        }

        public void Set(int idx, Vector3 position1, Vector3 position2, Color color)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            position1.Scale(scale * Vector3.one);
            position2.Scale(scale * Vector3.one);
            m_data[meshId].Set(lineId, position1, position2, color);
        }

        public void Set(int idx, Color color1, Color color2)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            m_data[meshId].Set(lineId, color1, color2);
        }

        public void Set(int idx, Color color)
        {
            CheckMesh(idx, out int meshId, out int lineId);
            m_data[meshId].Set(lineId, color);
        }

        public void Set(int idx, Vector3 position1, Vector3 position2)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            position1.Scale(scale * Vector3.one);
            position2.Scale(scale * Vector3.one);
            m_data[meshId].Set(pointId, position1, position2);
        }

        public void Set(int[] idxs, Vector3[] positions1, Vector3[] positions2, Color[] colors)
        {
            Debug.Assert((idxs.Length == positions1.Length) && (positions1.Length == positions2.Length) && (positions2.Length == colors.Length));
            for (int i = 0; i < idxs.Length; ++i)
            {
                Set(idxs[i], positions1[i], positions2[i], colors[i]);
            }
        }

        
    }
}
