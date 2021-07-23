using Elektronik.Clouds;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshContainerGpuRenderer : MonoBehaviour, IMeshRenderer
    {
        [SerializeField] private Shader MeshShader;
        [SerializeField] private int VertexCapacity = 65536;
        
        private Material _renderMaterial;
        private ComputeBuffer _vertsBuffer;
        private readonly int _vertsBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private GPUItem[] _vertices;
        private bool _meshUpdated = false;
        
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            lock (_vertices)
            {
                for (int i = 0; i < _vertices.Length; i++)
                {
                    if (i < e.Triangles.Length)
                    {
                        _vertices[i] = new GPUItem {Position = e.Vertices[e.Triangles[i]], Color = 10};
                    }
                    else
                    {
                        _vertices[i] = default;
                    }
                }  
                _meshUpdated = true;
            }
        }

        private void Start()
        {
            _vertices = new GPUItem[VertexCapacity];
            _vertsBuffer = new ComputeBuffer(_vertices.Length, GPUItem.Size);
            _renderMaterial = new Material(MeshShader) {hideFlags = HideFlags.DontSave};
            _renderMaterial.EnableKeyword("_COMPUTE_BUFFER");
        }

        private void Update()
        {
            if (!_meshUpdated) return;
            lock (_vertices)
            {
                _vertsBuffer.SetData(_vertices);
                _meshUpdated = false;
            }
        }

        private void OnDestroy()
        {
            _vertsBuffer.Dispose();
        }

        private void OnRenderObject()
        {
            _renderMaterial.SetPass(0);
            _renderMaterial.SetBuffer(_vertsBufferShaderProp, _vertsBuffer);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, _vertices.Length);
        }
    }
}