using Elektronik.Common.Clouds.Meshes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPointCloud : MonoBehaviour, IFastPointsCloud
    {
        public float scale = 1;

        public GameObject meshObjectPrefab;
        private ObjectPool m_meshObjectPool;
        private Dictionary<int, IPointsMeshObject> m_meshObjects;
        private int m_maxPointsCount;

        private void Awake()
        {
            m_meshObjects = new Dictionary<int, IPointsMeshObject>();
            m_maxPointsCount = meshObjectPrefab.GetComponent<IPointsMeshObject>().MaxPointsCount;
            m_meshObjectPool = new ObjectPool(meshObjectPrefab);
        }

        private void CheckMesh(int srcPointIdx, out int meshIdx, out int pointIdx)
        {
            meshIdx = srcPointIdx / m_maxPointsCount;
            if (!m_meshObjects.ContainsKey(meshIdx))
            {
                AddNewMesh(meshIdx);
            }
            pointIdx = srcPointIdx % m_maxPointsCount;
        }

        public bool Exists(int idx)
        {
            int meshIdx = idx / m_maxPointsCount;
            if (m_meshObjects.ContainsKey(meshIdx))
            {
                int pointIdx = idx % m_maxPointsCount;
                return m_meshObjects[meshIdx].Exists(pointIdx);
            }
            else
            {
                return false;
            }
        }

        public void Get(int idx, out Vector3 position, out Color color)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_meshObjects[meshId].Get(pointId, out position, out color);
        }

        public void Set(int idx, Matrix4x4 offset, Color color)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_meshObjects[meshId].Set(pointId, offset, color);
        }

        public void Set(int[] idxs, Matrix4x4[] offsets, Color[] colors)
        {
            Debug.Assert((idxs.Length == offsets.Length) && (offsets.Length == colors.Length));
            for (int i = 0; i < idxs.Length; ++i)
            {
                Set(idxs[i], offsets[i], colors[i]);
            }
        }

        public void Set(int idx, Color color)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_meshObjects[meshId].Set(pointId, color);
        }

        public void Clear()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Clear();
                m_meshObjectPool.Despawn(meshObjectPrefab);
            }
            m_meshObjects.Clear();
        }

        public void Repaint()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Repaint();
            }
        }

        public void GetAll(out int[] indices, out Vector3[] positions, out Color[] colors)
        {
            indices = Enumerable.Repeat(1, m_maxPointsCount * m_meshObjects.Count).ToArray();
            positions = new Vector3[m_maxPointsCount * m_meshObjects.Count];
            colors = new Color[m_maxPointsCount * m_meshObjects.Count];
            KeyValuePair<int, IPointsMeshObject>[] allMeshes = m_meshObjects.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                allMeshes[meshNum].Value.GetAll(out Vector3[] meshObjPositions, out Color[] meshObjColors);
                for (int i = 0; i < m_maxPointsCount; ++i)
                {
                    positions[m_maxPointsCount * allMeshes[meshNum].Key + i] = meshObjPositions[i];
                    colors[m_maxPointsCount * allMeshes[meshNum].Key + i] = meshObjColors[i];
                }
            }
        }

        private void AddNewMesh(int idx)
        {
            IPointsMeshObject newMesh = m_meshObjectPool.Spawn().GetComponent<IPointsMeshObject>();
            m_meshObjects.Add(idx, newMesh);
        }

        public void SetActive(bool value)
        {
            m_meshObjectPool.OnObjectSpawn += (o, s) => o.SetActive(value);
            foreach (var mesh in m_meshObjectPool.ActiveObject)
                mesh.SetActive(value);
        }
    }
}
