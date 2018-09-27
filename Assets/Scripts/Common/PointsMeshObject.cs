using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PointsMeshObject : MonoBehaviour
    {
        public const int MAX_VERTICES_COUNT = 65000;

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
            m_mesh.SetIndices(m_indices, MeshTopology.Points, 0);
            m_filter.mesh = m_mesh;
        }

        public bool PointExists(int idx)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_VERTICES_COUNT, "Wrong idx ({0})", idx.ToString());
            return (m_vertices[idx] == Vector3.zero && m_colors[idx] == new Color(0, 0, 0, 0));
        }

        public void GetPoint(int idx, out Vector3 position, out Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_VERTICES_COUNT, "Wrong idx ({0})", idx.ToString());
            position = m_vertices[idx];
            color = m_colors[idx];
        }

        public void SetPoint(int idx, Vector3 position, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_VERTICES_COUNT, "Wrong idx ({0})", idx.ToString());
            m_vertices[idx] = position;
            m_colors[idx] = color;
        }

        public void SetPointColor(int idx, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_VERTICES_COUNT, "Wrong idx ({0})", idx.ToString());
            m_colors[idx] = color;
        }

        public void SetPointPosition(int idx, Vector3 position)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_VERTICES_COUNT, "Wrong idx ({0})", idx.ToString());
            m_vertices[idx] = position;
        }

        public void Repaint()
        {
            m_mesh.vertices = m_vertices;
            m_mesh.colors = m_colors;
            m_mesh.RecalculateBounds();
        }

        public void GetAllPoints(out Vector3[] positions, out Color[] colors)
        {
            positions = m_vertices.Clone() as Vector3[];
            colors = m_colors.Clone() as Color[];
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