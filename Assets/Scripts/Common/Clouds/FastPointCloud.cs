using Elektronik.Common.Clouds.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPointCloud : MonoBehaviour, IFastPointsCloud
    {
        public float scale = 1;

        public GameObject meshObjectPrefab;
        private IPointsMeshObject m_pointsMesh;
        private Dictionary<int, IPointsMeshObject> m_meshObjects;

        private void Awake()
        {
            m_meshObjects = new Dictionary<int, IPointsMeshObject>();
            m_pointsMesh = meshObjectPrefab.GetComponent<IPointsMeshObject>();
        }

        private void CheckMesh(int srcPointIdx, out int meshIdx, out int pointIdx)
        {
            meshIdx = srcPointIdx / m_pointsMesh.MaxPointsCount;
            if (!m_meshObjects.ContainsKey(meshIdx))
            {
                AddNewMesh(meshIdx);
            }
            pointIdx = srcPointIdx % m_pointsMesh.MaxPointsCount;
        }
        
        public bool Exists(int idx)
        {
            int meshIdx = idx / m_pointsMesh.MaxPointsCount;
            if (m_meshObjects.ContainsKey(meshIdx))
            {
                int pointIdx = idx % m_pointsMesh.MaxPointsCount;
                return m_meshObjects[meshIdx].Exists(pointIdx);
            }
            else
            {
                return false;
            }
        }

        public void Get(int idx, out Vector3 position, out Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].Get(pointId, out position, out color);
        }

        public void Set(int idx, Matrix4x4 offset, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
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
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].Set(pointId, color);
        }

        public void Clear()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Clear();
                MF_AutoPool.Despawn(meshObjectPrefab);
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
            indices = Enumerable.Repeat(1, m_pointsMesh.MaxPointsCount * m_meshObjects.Count).ToArray();
            positions = new Vector3[m_pointsMesh.MaxPointsCount * m_meshObjects.Count];
            colors = new Color[m_pointsMesh.MaxPointsCount * m_meshObjects.Count];
            KeyValuePair<int, IPointsMeshObject>[] allMeshes = m_meshObjects.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                Vector3[] meshObjPositions;
                Color[] meshObjColors;
                allMeshes[meshNum].Value.GetAll(out meshObjPositions, out meshObjColors);
                for (int i = 0; i < m_pointsMesh.MaxPointsCount; ++i)
                {
                    positions[m_pointsMesh.MaxPointsCount * allMeshes[meshNum].Key + i] = meshObjPositions[i];
                    colors[m_pointsMesh.MaxPointsCount * allMeshes[meshNum].Key + i] = meshObjColors[i];
                }
            }
        }

        private void AddNewMesh(int idx)
        {
            IPointsMeshObject newMesh = MF_AutoPool.Spawn(meshObjectPrefab).GetComponent<IPointsMeshObject>();
            m_meshObjects.Add(idx, newMesh);
        }

        public void SetActive(bool value)
        {
            meshObjectPrefab.SetActive(value);
            MF_AutoPool.ForEach(meshObjectPrefab, obj => obj.SetActive(value));
        }
    }
}
