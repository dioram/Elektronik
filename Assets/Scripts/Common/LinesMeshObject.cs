using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class LinesMeshObject : MonoBehaviour
    {
        public const int MAX_VERTICES_COUNT = 65000;
        public const int MAX_LINES_COUNT = MAX_VERTICES_COUNT / 2;

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
            m_mesh.SetIndices(m_indices, MeshTopology.Lines, 0);
            m_filter.mesh = m_mesh;
        }

        public bool LineExists(int idx)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_LINES_COUNT, "Wrong idx ({0})", idx.ToString());
            Vector3 pos1;
            Vector3 pos2;
            Color color;
            GetLine(idx, out pos1, out pos2, out color);
            return (pos1 == Vector3.zero && pos2 == Vector3.zero && color == new Color(0, 0, 0, 0));
        }

        public void GetLine(int idx, out Vector3 position1, out Vector3 position2, out Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_LINES_COUNT, "Wrong idx ({0})", idx.ToString());
            position1 = m_vertices[2 * idx];
            position2 = m_vertices[2 * idx + 1];
            color = m_colors[2 * idx];
        }

        public void SetLine(int idx, Vector3 position1, Vector3 position2, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_LINES_COUNT / 2, "Wrong idx ({0})", idx.ToString());
            SetLineColor(idx, color);
            SetLinePositions(idx, position1, position2);
        }

        public void SetLineColor(int idx, Color color)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_LINES_COUNT / 2, "Wrong idx ({0})", idx.ToString());
            m_colors[2 * idx] = color;
            m_colors[2 * idx + 1] = color;
        }

        public void SetLinePositions(int idx, Vector3 position1, Vector3 position2)
        {
            Debug.AssertFormat(idx >= 0 && idx < MAX_LINES_COUNT / 2, "Wrong idx ({0})", idx.ToString());
            m_vertices[2 * idx] = position1;
            m_vertices[2 * idx + 1] = position2;
        }

        public void Repaint()
        {
            m_mesh.vertices = m_vertices;
            m_mesh.colors = m_colors;
            m_mesh.RecalculateBounds();
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
