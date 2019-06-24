using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PointsMeshObject : MonoBehaviour, IPointsMeshObject
    {
        private const int MAX_VERTICES_COUNT = 65000;

        private MeshFilter m_filter;
        private Mesh m_mesh;

        private int[] m_indices;
        private Vector3[] m_vertices;
        private Color[] m_colors;
        private bool m_needReapint = false;
        public int MaxPointsCount => MAX_VERTICES_COUNT;

        private void Awake()
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

        public void Get(int idx, out Vector3 position, out Color color)
        {
            Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
            position = m_vertices[idx];
            color = m_colors[idx];
        }

        public void Set(int idx, Matrix4x4 position, Color color)
        {
            Set(idx, color);
            Set(idx, position);
        }

        public void Set(int idx, Color color)
        {
            Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
            m_colors[idx] = color;
            m_needReapint = true;
        }

        public void Set(int idx, Matrix4x4 position)
        {
            Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
            m_vertices[idx] = position.GetPosition();
            m_needReapint = true;
        }

        public void Repaint()
        {
            if (m_needReapint)
            {
                m_mesh.vertices = m_vertices;
                m_mesh.colors = m_colors;
                m_mesh.RecalculateBounds();
                m_needReapint = false;
            }
        }

        public void GetAll(out Vector3[] positions, out Color[] colors)
        {
            positions = m_vertices.Clone() as Vector3[];
            colors = m_colors.Clone() as Color[];
        }

        public void Clear()
        {
            for (int i = 0; i < m_vertices.Length; ++i)
            {
                m_vertices[i] = Vector3.zero;
                m_colors[i] = new Color(0, 0, 0, 0);
            }
            Repaint();
        }

        public bool Exists(int idx)
        {
            Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
            return !(m_vertices[idx] == Vector3.zero && m_colors[idx] == new Color(0, 0, 0, 0));
        }
    }
}