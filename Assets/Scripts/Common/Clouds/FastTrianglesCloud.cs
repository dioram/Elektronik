using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastTrianglesCloud : MonoBehaviour
    {
        public float scale = 1;

        public TrianglesMeshObject meshObjectPrefab;
        private Dictionary<int, TrianglesMeshObject> m_meshObjects;

        private void Awake()
        {
            m_meshObjects = new Dictionary<int, TrianglesMeshObject>();
        }

        private void CheckMesh(int srcPointIdx, out int meshIdx, out int pointIdx)
        {
            meshIdx = srcPointIdx / TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT;
            if (!m_meshObjects.ContainsKey(meshIdx))
            {
                AddNewMesh(meshIdx);
            }
            pointIdx = srcPointIdx % TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT;
        }

        public bool TetrahedronExists(int idx)
        {
            int meshIdx = idx / TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT;
            if (m_meshObjects.ContainsKey(meshIdx))
            {
                int pointIdx = idx % TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT;
                return m_meshObjects[meshIdx].TetrahedronExists(pointIdx);
            }
            else
            {
                return false;
            }
        }

        public void GetTetrahedron(int idx, out Vector3 position, out Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].GetTetrahedron(pointId, out position, out color);
        }


        public void SetTetrahedron(int idx, Vector3 vertix, Quaternion rotation, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetTetrahedron(pointId, vertix * scale, rotation, color);
        }

        public void SetTetrahedron(int idx, Vector3 vertix, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetTetrahedron(pointId, vertix * scale, color);
        }

        public void SetTetrahedron(int idx, Quaternion rotation, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetTetrahedron(pointId, rotation, color);
        }

        public void SetTetrahedron(int idx, Color color)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetTetrahedron(pointId, color);
        }

        public void SetTetrahedron(int idx, Vector3 position)
        {
            int meshId;
            int pointId;
            CheckMesh(idx, out meshId, out pointId);
            m_meshObjects[meshId].SetTetrahedron(pointId, position * scale);
        }

        public void SetTetrahedrons(int[] idxs, Vector3[] vertices, Color[] colors)
        {
            Debug.Assert((idxs.Length == vertices.Length) && (vertices.Length == colors.Length));
            for (int i = 0; i < idxs.Length; ++i)
            {
                SetTetrahedron(idxs[i], vertices[i], colors[i]);
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

        public void GetAllTetrahedrons(out int[] indices, out Vector3[] positions, out Color[] colors)
        {
            indices = Enumerable.Repeat(1, TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT * m_meshObjects.Count).ToArray();
            positions = new Vector3[TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT * m_meshObjects.Count];
            colors = new Color[TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT * m_meshObjects.Count];
            KeyValuePair<int, TrianglesMeshObject>[] allMeshes = m_meshObjects.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                Vector3[] meshObjPositions;
                Color[] meshObjColors;
                allMeshes[meshNum].Value.GetAllTetrahedrons(out meshObjPositions, out meshObjColors);
                for (int i = 0; i < TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT; ++i)
                {
                    positions[TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT * allMeshes[meshNum].Key + i] = meshObjPositions[i];
                    colors[TrianglesMeshObject.MAX_THETRAHEDRONS_COUNT * allMeshes[meshNum].Key + i] = meshObjColors[i];
                }
            }
        }

        private void AddNewMesh(int idx)
        {
            TrianglesMeshObject newMesh = MF_AutoPool.Spawn(meshObjectPrefab.gameObject).GetComponent<TrianglesMeshObject>();
            m_meshObjects.Add(idx, newMesh);
        }
    }
}
