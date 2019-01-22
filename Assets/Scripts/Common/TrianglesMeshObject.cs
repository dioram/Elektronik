using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class TrianglesMeshObject : MonoBehaviour
    {
        private const int INDICES_PER_THETRAHEDRON = 12;
        private const int MAX_VERTICES_COUNT = 64992;
        public const int MAX_THETRAHEDRONS_COUNT = MAX_VERTICES_COUNT / INDICES_PER_THETRAHEDRON;
        
        public float sideSize = .001f;

        MeshFilter m_filter;
        Mesh m_mesh;

        int[] m_indices;
        Vector3[] m_vertices;
        Color[] m_colors;

        void Awake()
        {
            
            m_indices = new int[MAX_VERTICES_COUNT];
            for (int i = 0; i < MAX_VERTICES_COUNT; ++i) // 12 is count of vertices of thetrahedron
            {
                m_indices[i] = i;
            }
            
            m_vertices = new Vector3[MAX_VERTICES_COUNT];
            m_colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            m_filter = GetComponent<MeshFilter>();
            if (m_filter.mesh == null)
            {
                m_filter.mesh = new Mesh();
            }
            m_mesh = m_filter.sharedMesh;
        }

        private void Start()
        {
            m_mesh.MarkDynamic();
            m_mesh.vertices = m_vertices;
            m_mesh.colors = m_colors;
            m_mesh.SetIndices(m_indices, MeshTopology.Triangles, 0);
            m_filter.mesh = m_mesh;
        }

        public bool TriangleExists(int idx)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, "Wrong idx ({0})", idx.ToString());
            bool verticesIsDefault = true;
            verticesIsDefault = verticesIsDefault && (m_vertices[idx * 4] == m_vertices[idx * 4 + 1]);
            verticesIsDefault = verticesIsDefault && (m_vertices[idx * 4 + 1] == m_vertices[idx * 4 + 2]);
            return verticesIsDefault;
        }

        public void GetTriangle(int idx, out Vector3 triangleCG, out Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, "Wrong idx ({0})", idx.ToString());
            triangleCG = new Vector3();
            for (int i = 0; i < INDICES_PER_THETRAHEDRON; ++i)
            {
                triangleCG += m_vertices[idx * INDICES_PER_THETRAHEDRON + i];
            }
            triangleCG /= INDICES_PER_THETRAHEDRON;
            color = m_colors[idx * INDICES_PER_THETRAHEDRON];
        }

        public void SetTriangle(int idx, Vector3 triangleCG, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, "Wrong idx ({0})", idx.ToString());
            SetTrianglePosition(idx, triangleCG);
            SetTriangleColor(idx, color);
        }

        public void SetTriangleColor(int idx, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, "Wrong idx ({0})", idx.ToString());
            for (int i = 0; i < INDICES_PER_THETRAHEDRON; ++i)
            {
                m_colors[idx * INDICES_PER_THETRAHEDRON + i] = color;
            }
        }

        public void SetTrianglePosition(int idx, Vector3 CG)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, "Wrong idx ({0})", idx.ToString());
            float halfOfSideSize = sideSize / 2f;
            Vector3 toCenter = new Vector3() * (-halfOfSideSize);
            Vector3 v0 = new Vector3(0, 0, 0) + toCenter + CG;
            Vector3 v1 = new Vector3(sideSize, 0, 0) + toCenter + CG;
            Vector3 v2 = new Vector3(sideSize / 2, 0, 0.86603f * sideSize) + toCenter + CG;
            Vector3 v3 = new Vector3(sideSize / 2, 0.86603f * sideSize, 0.86603f * sideSize / 3) + toCenter + CG;

            m_vertices[idx * INDICES_PER_THETRAHEDRON + 0] = v0;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 1] = v1;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 2] = v2;

            m_vertices[idx * INDICES_PER_THETRAHEDRON + 3] = v0;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 4] = v2;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 5] = v3;

            m_vertices[idx * INDICES_PER_THETRAHEDRON + 6] = v2;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 7] = v1;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 8] = v3;

            m_vertices[idx * INDICES_PER_THETRAHEDRON + 9] = v0;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 10] = v3;
            m_vertices[idx * INDICES_PER_THETRAHEDRON + 11] = v1;


        }

        public void Repaint()
        {
            m_mesh.vertices = m_vertices;
            m_mesh.colors = m_colors;
            m_mesh.RecalculateNormals();
            m_mesh.RecalculateBounds();
        }

        public void GetAllTriangles(out Vector3[] trianglesCGs, out Color[] colors)
        {
            List<Vector3> trianglesCGs_ = new List<Vector3>(MAX_THETRAHEDRONS_COUNT);
            List<Color> trianglesColors_ = new List<Color>(MAX_THETRAHEDRONS_COUNT);
            for (int i = 0; i < MAX_THETRAHEDRONS_COUNT; ++i)
            {
                if (TriangleExists(i))
                {
                    Vector3 CG = new Vector3();
                    Color color = new Color();
                    GetTriangle(i, out CG, out color);
                    trianglesCGs_.Add(CG);
                    trianglesColors_.Add(color);
                }
            }
            trianglesCGs = trianglesCGs_.ToArray();
            colors = trianglesColors_.ToArray();
        }

        public void Clear()
        {
            for (int i = 0; i < MAX_THETRAHEDRONS_COUNT; ++i)
            {
                m_vertices[i] = Vector3.zero;
                m_colors[i] = new Color(0, 0, 0, 0);
            }

            Repaint();
        }
    }
}
