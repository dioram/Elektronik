using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public class FastPointCloud : MonoBehaviour
    {
        public float scale = 1;

        public PointsMeshObject meshObjectPrefab;
        private Dictionary<int, PointsMeshObject> m_meshObjects;

        private void Awake()
        {
            m_meshObjects = new Dictionary<int, PointsMeshObject>();
        }

        private void CheckMesh(int srcPointIdx, out int meshIdx, out int pointIdx)
        {
            meshIdx = srcPointIdx / PointsMeshObject.MAX_VERTICES_COUNT;
            if (!m_meshObjects.ContainsKey(meshIdx))
            {
                AddNewMesh(meshIdx);
            }
            pointIdx = srcPointIdx % PointsMeshObject.MAX_VERTICES_COUNT;
        }
        
        public bool PointExists(int idx)
        {
            int meshIdx = idx / PointsMeshObject.MAX_VERTICES_COUNT;
            if (m_meshObjects.ContainsKey(meshIdx))
            {
                int pointIdx = idx % PointsMeshObject.MAX_VERTICES_COUNT;
                return m_meshObjects[meshIdx].PointExists(pointIdx);
            }
            else
            {
                return false;
            }
        }

        public void GetPoint(int idx, out Vector3 position, out Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].GetPoint(pointId, out position, out color);
        }

        public void SetPoint(int idx, Vector3 vertix, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetPoint(pointId, vertix * scale, color);
        }

        public void SetPointColor(int idx, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetPointColor(pointId, color);
        }

        public void SetPointPosition(int idx, Vector3 position)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetPointPosition(pointId, position * scale);
        }

        public void SetPoints(int[] idxs, Vector3[] vertices, Color[] colors)
        {
            Debug.Assert((idxs.Length == vertices.Length) && (vertices.Length == colors.Length));
            for (int i = 0; i < idxs.Length; ++i)
            {
                SetPoint(idxs[i], vertices[i], colors[i]);
            }
        }

        public void Clear()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Clear();
                MF_AutoPool.Despawn(meshObject.Value.gameObject);
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

        public void GetAllPoints(out int[] indices, out Vector3[] positions, out Color[] colors)
        {
            indices = Enumerable.Repeat(1, PointsMeshObject.MAX_VERTICES_COUNT * m_meshObjects.Count).ToArray();
            positions = new Vector3[PointsMeshObject.MAX_VERTICES_COUNT * m_meshObjects.Count];
            colors = new Color[PointsMeshObject.MAX_VERTICES_COUNT * m_meshObjects.Count];
            KeyValuePair<int, PointsMeshObject>[] allMeshes = m_meshObjects.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                Vector3[] meshObjPositions;
                Color[] meshObjColors;
                allMeshes[meshNum].Value.GetAllPoints(out meshObjPositions, out meshObjColors);
                for (int i = 0; i < PointsMeshObject.MAX_VERTICES_COUNT; ++i)
                {
                    positions[PointsMeshObject.MAX_VERTICES_COUNT * allMeshes[meshNum].Key + i] = meshObjPositions[i];
                    colors[PointsMeshObject.MAX_VERTICES_COUNT * allMeshes[meshNum].Key + i] = meshObjColors[i];
                }
            }
        }

        private void AddNewMesh(int idx)
        {
            PointsMeshObject newMesh = MF_AutoPool.Spawn(meshObjectPrefab.gameObject).GetComponent<PointsMeshObject>();
            m_meshObjects.Add(idx, newMesh);
        }
    }
}
