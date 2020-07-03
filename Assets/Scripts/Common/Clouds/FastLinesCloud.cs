using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;


namespace Elektronik.Common.Clouds
{
    public class FastLinesCloud : MonoBehaviour, IFastLinesCloud
    {
        public float scale = 1;

        public LinesMeshObjectBase meshObjectPrefab;
        private ObjectPool m_meshObjectPool;
        private Dictionary<int, ILinesMeshData> m_data;
        private Dictionary<ILinesMeshData, MeshObjectBase<ILinesMeshData>> m_meshObjects;
        private Queue<MeshDataBase<ILinesMeshData>> m_newMeshQueue;
        private Queue<ILinesMeshData> m_removedMeshQueue;
        private int m_maxLinesCount;

        private void Awake()
        {
            m_meshObjectPool = new ObjectPool(meshObjectPrefab.gameObject);
            m_data = new Dictionary<int, ILinesMeshData>();
            m_meshObjects = new Dictionary<ILinesMeshData, MeshObjectBase<ILinesMeshData>>();
            m_newMeshQueue = new Queue<MeshDataBase<ILinesMeshData>>();
            m_removedMeshQueue = new Queue<ILinesMeshData>();
            m_maxLinesCount = meshObjectPrefab.MaxObjectsCount;
        }

        private void CheckMesh(int srcLineIdx, out int meshIdx, out int lineIdx)
        {
            meshIdx = srcLineIdx / m_maxLinesCount;
            if (!m_data.ContainsKey(meshIdx))
            {
                var data = meshObjectPrefab.CreateMeshData();
                m_data[meshIdx] = data.Data;
                lock (m_newMeshQueue) m_newMeshQueue.Enqueue(data);
            }
            lineIdx = srcLineIdx % m_maxLinesCount;
        }

        public void Clear()
        {
            foreach (var meshData in m_data)
            {
                lock (m_removedMeshQueue) m_removedMeshQueue.Enqueue(meshData.Value);
            }
            m_data.Clear();
        }

        private void Update()
        {
            lock (m_newMeshQueue)
            {
                while (m_newMeshQueue.Count != 0)
                {
                    var meshDataBase = m_newMeshQueue.Dequeue();
                    m_meshObjects[meshDataBase.Data] = m_meshObjectPool.Spawn().GetComponent<MeshObjectBase<ILinesMeshData>>();
                    m_meshObjects[meshDataBase.Data].Initialize(meshDataBase);
                }
            }
            lock (m_removedMeshQueue)
            {
                while (m_removedMeshQueue.Count != 0)
                {
                    var meshDataBase = m_removedMeshQueue.Dequeue();
                    m_meshObjectPool.Despawn(m_meshObjects[meshDataBase].gameObject);
                    m_meshObjects.Remove(meshDataBase);
                }
            }
        }

        public bool Exists(int lineIdx)
        {
            int meshIdx = lineIdx / m_maxLinesCount;
            if (m_data.ContainsKey(meshIdx))
            {
                return m_data[meshIdx].Exists(lineIdx % m_maxLinesCount);
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

        public void SetActive(bool value)
        {
            m_meshObjectPool.SetActive(value);
        }
    }
}
