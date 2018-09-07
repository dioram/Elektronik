using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class MeshObject : MonoBehaviour
    {
        const int MAX_VERTICES_COUNT = 65000;

        MeshRenderer m_renderer;
        MeshFilter m_filter;
        int[] m_indices;
        Vector3[] m_vertices;
        Color[] m_colors;

        // Use this for initialization
        void Awake()
        {
            m_renderer = GetComponent<MeshRenderer>();
            m_filter = GetComponent<MeshFilter>();
            m_indices = Enumerable.Range(0, MAX_VERTICES_COUNT).ToArray();
            m_vertices = new Vector3[MAX_VERTICES_COUNT];
            m_colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            m_filter.mesh = new Mesh();
            m_filter.mesh.vertices = m_vertices;
            m_filter.mesh.colors = m_colors;
            m_filter.mesh.SetIndices(m_indices, MeshTopology.Points, 0);
        }

        public void SetPoint(int idx, Vector3 pos, Color color)
        {
            Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT);
            m_vertices[idx] = pos;
            m_colors[idx] = color;
        }

        public void Repaint()
        {
            m_filter.mesh.RecalculateBounds();
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