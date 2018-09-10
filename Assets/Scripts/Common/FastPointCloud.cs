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

        public MeshObject meshObjectPrefab;
        private Dictionary<int, MeshObject> m_meshObjects;

        private void Awake()
        {
            m_meshObjects = new Dictionary<int, MeshObject>();
        }

        private void CheckMesh(int srcPointIdx, out int meshIdx, out int pointIdx)
        {
            meshIdx = srcPointIdx / MeshObject.MAX_VERTICES_COUNT;
            if (!m_meshObjects.ContainsKey(meshIdx))
            {
                AddNewMesh(meshIdx);
            }
            pointIdx = srcPointIdx % MeshObject.MAX_VERTICES_COUNT;
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
            }
            MF_AutoPool.DespawnPool(meshObjectPrefab.gameObject);
            m_meshObjects.Clear();
        }

        public void Repaint()
        {
            foreach (var meshObject in m_meshObjects)
            {
                meshObject.Value.Repaint();
            }
        }


        private void AddNewMesh(int idx)
        {
            MeshObject newMesh = MF_AutoPool.Spawn(meshObjectPrefab.gameObject).GetComponent<MeshObject>();
            m_meshObjects.Add(idx, newMesh);
        }
    }
}
