using Elektronik.Clouds;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshContainerGpuRenderer : MonoBehaviour, IMeshRenderer
    {
        [SerializeField] private Shader MeshShaderUnlit;
        [SerializeField] private Shader MeshShaderLit;
        private const int VertexCapacity = 262144;
        public bool OverrideColors = false;
        
        private Material _renderMaterialLit;
        private Material _renderMaterialUnlit;
        private ComputeBuffer _vertsBuffer;
        private readonly int _vertsBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");
        private GPUItem[] _vertices;
        private bool _meshUpdated = false;

        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            lock (_vertices)
            {
                for (var i = 0; i < _vertices.Length; i++)
                {
                    if (i < e.Triangles.Length)
                    {
                        _vertices[i] = new GPUItem(e.Vertices[e.Triangles[i]]);
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
            _renderMaterialUnlit = new Material(MeshShaderUnlit) {hideFlags = HideFlags.DontSave};
            _renderMaterialUnlit.EnableKeyword("_COMPUTE_BUFFER");
            _renderMaterialLit = new Material(MeshShaderLit) {hideFlags = HideFlags.DontSave};
            _renderMaterialLit.EnableKeyword("_COMPUTE_BUFFER");
        }

        private void Update()
        {
            if (!_meshUpdated) return;
            lock (_vertices)
            {
                lock (_vertsBuffer)
                {
                    _vertsBuffer.SetData(_vertices);
                }
                _meshUpdated = false;
            }
        }

        private void OnDestroy()
        {
            lock (_vertsBuffer)
            {
                _vertsBuffer.Dispose();
            }
        }

        private void OnRenderObject()
        {
            var renderMat = OverrideColors ? _renderMaterialUnlit : _renderMaterialLit;
            renderMat.SetPass(0);
            lock (_vertsBuffer)
            {
                renderMat.SetBuffer(_vertsBufferShaderProp, _vertsBuffer);
            }
            Graphics.DrawProceduralNow(MeshTopology.Triangles, _vertices.Length);
        }

        public void SetScale(float value)
        {
            _renderMaterialUnlit.SetFloat(_scaleShaderProp, value);
            _renderMaterialLit.SetFloat(_scaleShaderProp, value);
        }
    }
}