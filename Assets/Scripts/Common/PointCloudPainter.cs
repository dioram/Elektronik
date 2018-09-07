using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Elektronik.Common
{
    public class PointCloudPainter : MonoBehaviour
    {
        const int MAX_VERTICES_COUNT = 65000;

        public GameObject meshObjectPrefab;

        private List<MeshObject> m_paintedMeshes;

        public PointCloudPainter()
        {
            m_paintedMeshes = new List<MeshObject>();
        }

        public void DrawCloud(Vector3[] points, Color[] colors)
        {
            Debug.Assert(points.Length == colors.Length);
            List<int> meshSizes = Enumerable.Repeat(MAX_VERTICES_COUNT, points.Length / MAX_VERTICES_COUNT).ToList();
            int rest = points.Length % MAX_VERTICES_COUNT;
            if (rest != 0)
            {
                meshSizes.Add(rest);
            }
            m_paintedMeshes = new List<MeshObject>(meshSizes.Count);
            for (int i = 0; i < meshSizes.Count; ++i)
            {
                MeshObject meshObject = MF_AutoPool.Spawn(meshObjectPrefab).GetComponent<MeshObject>();
                m_paintedMeshes.Add(meshObject);
                for (int j = 0; j < meshSizes[i]; ++j)
                {
                    meshObject.SetPoint(j, points[i * MAX_VERTICES_COUNT + j], colors[i * MAX_VERTICES_COUNT + j]);
                }
                meshObject.Repaint();
            }
        }

        public void Clear()
        {
            foreach (var mesh in m_paintedMeshes)
            {
                mesh.Clear();
            }
            m_paintedMeshes.Clear();
            MF_AutoPool.DespawnPool(meshObjectPrefab);
        }

        // Use this for initialization
        void Start()
        {

        }

    }
}