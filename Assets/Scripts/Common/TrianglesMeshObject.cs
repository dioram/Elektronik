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
        public const int MAX_VERTICES_COUNT = 64998;
        public const int MAX_TRIANGLES_COUNT = MAX_VERTICES_COUNT / 3;
        public float triangleSideSize = .2f;

        MeshFilter m_filter;
        Mesh m_mesh;

        int[] m_indices;
        Vector3[] m_vertices;
        Color[] m_colors;

        void Awake()
        {
            m_filter = GetComponent<MeshFilter>();
            m_indices = Enumerable.Range(0, MAX_VERTICES_COUNT).ToArray();
            m_vertices = new Vector3[MAX_VERTICES_COUNT];
            m_colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            m_mesh = new Mesh();
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
            Debug.AssertFormat(idx >= 0 && idx < MAX_TRIANGLES_COUNT, "Wrong idx ({0})", idx.ToString());
            bool verticesIsDefault = true;
            verticesIsDefault = verticesIsDefault && (m_vertices[idx * 3] == m_vertices[idx * 3 + 1]);
            verticesIsDefault = verticesIsDefault && (m_vertices[idx * 3 + 1] == m_vertices[idx * 3 + 2]);
            return verticesIsDefault;
        }

        public void GetTriangle(int idx, out Vector3 triangleCG, out Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_TRIANGLES_COUNT, "Wrong idx ({0})", idx.ToString());
            Vector3 vert1 = m_vertices[idx * 3];
            Vector3 vert2 = m_vertices[idx * 3 + 1];
            Vector3 vert3 = m_vertices[idx * 3 + 2];
            triangleCG = (vert1 + vert2 + vert3) / 3;
            color = m_colors[idx * 3];
        }

        public void SetTriangle(int idx, Vector3 triangleCG, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_TRIANGLES_COUNT, "Wrong idx ({0})", idx.ToString());
            SetTrianglePosition(idx, triangleCG);
            SetTriangleColor(idx, color);
        }

        public void SetTriangleColor(int idx, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_TRIANGLES_COUNT, "Wrong idx ({0})", idx.ToString());
            m_colors[idx * 3] = color;
            m_colors[idx * 3 + 1] = color;
            m_colors[idx * 3 + 2] = color;
        }

        public void SetTrianglePosition(int idx, Vector3 triangleCG)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_TRIANGLES_COUNT, "Wrong idx ({0})", idx.ToString());
            float halfOfSideSize = triangleSideSize / 2f;
            m_vertices[idx * 3] = new Vector3(-halfOfSideSize, -halfOfSideSize) + triangleCG;
            m_vertices[idx * 3 + 1] = new Vector3(halfOfSideSize, -halfOfSideSize) + triangleCG;
            m_vertices[idx * 3 + 2] = new Vector3(0, 0.86603f * triangleSideSize - halfOfSideSize) + triangleCG;
        }

        public void Repaint()
        {
            m_mesh.vertices = m_vertices;
            m_mesh.colors = m_colors;
            m_mesh.RecalculateBounds();
        }

        public void GetAllTriangles(out Vector3[] trianglesCGs, out Color[] colors)
        {
            List<Vector3> trianglesCGs_ = new List<Vector3>(MAX_TRIANGLES_COUNT);
            List<Color> trianglesColors_ = new List<Color>(MAX_TRIANGLES_COUNT);
            for (int i = 0; i < MAX_TRIANGLES_COUNT; ++i)
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
            for (int i = 0; i < MAX_VERTICES_COUNT; ++i)
            {
                m_vertices[i] = Vector3.zero;
                m_colors[i] = new Color(0, 0, 0, 0);
            }
            Repaint();
        }
    }
}
