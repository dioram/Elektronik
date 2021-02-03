using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public abstract class MeshObjectBase<DataInterface> : MonoBehaviour
    {
        private MeshFilter m_filter;
        protected Mesh m_mesh;

        private MeshDataBase<DataInterface> m_meshData;
        public DataInterface Data { get => m_meshData.Data; }
        public abstract int MaxObjectsCount { get; }
        public abstract MeshDataBase<DataInterface> CreateMeshData();

        public void Initialize(MeshDataBase<DataInterface> data)
        {
            m_filter = GetComponent<MeshFilter>();
            if (m_filter.mesh == null)
            {
                m_filter.mesh = new Mesh();
            }
            m_mesh = m_filter.mesh;
            m_meshData = data;
            m_mesh.MarkDynamic();
            m_mesh.vertices = m_meshData.Vertices;
            m_mesh.colors = m_meshData.Colors;
            m_mesh.SetIndices(m_meshData.Indices, MeshTopology, 0);
            m_filter.mesh = m_mesh;
        }

        protected abstract MeshTopology MeshTopology { get; }

        public void Update()
        {
            if (m_meshData == null)
                return;
            if (m_meshData.HasChanged)
            {
                try
                {
                    m_meshData.Sync.EnterReadLock();
                    m_mesh.vertices = m_meshData.Vertices;
                    m_mesh.colors = m_meshData.Colors;

                    if (m_meshData.Normals != null) m_mesh.normals = m_meshData.Normals;
                    
                    if (MeshTopology == MeshTopology.Triangles)
                    {
                        m_mesh.RecalculateTangents();
                    }
                    m_mesh.RecalculateBounds();
                }
                finally
                {
                    m_meshData.Sync.ExitReadLock();
                }
            }
        }
    }
}
