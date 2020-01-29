using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class LinesMeshObject : MonoBehaviour
    {
        public const int MAX_VERTICES_COUNT = 65000;
        public const int MAX_LINES_COUNT = MAX_VERTICES_COUNT / 2;

        private MeshFilter m_filter;
        private Mesh m_mesh;

        private int[] m_indices;
        private Vector3[] m_vertices;
        private Color[] m_colors;
        private bool m_needRepaint;

        private void Awake()
        {
            m_filter = GetComponent<MeshFilter>();
            m_indices = Enumerable.Range(0, MAX_VERTICES_COUNT).ToArray();
            m_vertices = new Vector3[MAX_VERTICES_COUNT];
            m_colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            m_mesh = new Mesh();
            m_needRepaint = false;
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
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            GetLine(idx, out Vector3 pos1, out Vector3 pos2, out Color color);
            return !(pos1 == Vector3.zero && pos2 == Vector3.zero && color == new Color(0, 0, 0, 0));
        }

        public void GetLine(int idx, out Vector3 position1, out Vector3 position2, out Color color)
        {
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            position1 = m_vertices[2 * idx];
            position2 = m_vertices[2 * idx + 1];
            color = m_colors[2 * idx];
        }

        public void SetLine(int idx, Vector3 position1, Vector3 position2, Color color1, Color color2)
        {
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            SetLineColor(idx, color1, color2);
            SetLinePositions(idx, position1, position2);
            m_needRepaint = true;
        }

        public void SetLine(int idx, Vector3 position1, Vector3 position2, Color color)
        {
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            SetLineColor(idx, color);
            SetLinePositions(idx, position1, position2);
            m_needRepaint = true;
        }

        public void SetLineColor(int idx, Color color1, Color color2)
        {
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            m_colors[2 * idx] = color1;
            m_colors[2 * idx + 1] = color2;
            m_needRepaint = true;
        }

        public void SetLineColor(int idx, Color color)
        {
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            m_colors[2 * idx] = color;
            m_colors[2 * idx + 1] = color;
            m_needRepaint = true;
        }

        public void SetLinePositions(int idx, Vector3 position1, Vector3 position2)
        {
            Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
            m_vertices[2 * idx] = position1;
            m_vertices[2 * idx + 1] = position2;
            m_needRepaint = true;
        }

        public void Repaint()
        {
            if (m_needRepaint)
            {
                m_mesh.vertices = m_vertices;
                m_mesh.colors = m_colors;
                m_mesh.RecalculateBounds();
                m_needRepaint = false;
            }
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
