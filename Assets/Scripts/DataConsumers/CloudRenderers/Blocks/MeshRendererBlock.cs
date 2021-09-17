using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MeshRendererBlock : CloudBlock
    {
        public Shader CloudShaderLit;
        public bool OverrideColors = false;
        public GPUItem[] Vertices;
        
        public new const int Capacity = 256 * 256 * 3;

        public override GPUItem[] GetItems() => Vertices;

        public override void SetScale(float value)
        {
            _renderMaterialLit.SetFloat(_scaleShaderProp, value);
            _renderMaterialUnlit.SetFloat(_scaleShaderProp, value);
        }

        #region Unity events

        protected override void Start()
        {
            _renderMaterialUnlit = new Material(CloudShader) {hideFlags = HideFlags.DontSave};
            _renderMaterialUnlit.EnableKeyword("_COMPUTE_BUFFER");
            _renderMaterialLit = new Material(CloudShaderLit) {hideFlags = HideFlags.DontSave};
            _renderMaterialLit.EnableKeyword("_COMPUTE_BUFFER");
        }
        
        protected override void OnRenderObject()
        {
            var renderMat = OverrideColors ? _renderMaterialLit : _renderMaterialUnlit;
            renderMat.SetPass(0);
            SendData(renderMat);
            Draw();
        }

        #endregion

        #region Protected
        protected override void Init()
        {
            Vertices = Enumerable.Repeat(default(GPUItem), Capacity).ToArray();
            _vertexBuffer = new ComputeBuffer(Vertices.Length, GPUItem.Size);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetBuffer(_vertexBufferShaderProp, _vertexBuffer);
        }

        protected override void OnUpdated()
        {
             _vertexBuffer.SetData(Vertices);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Triangles, Vertices.Length);
        }

        protected override void ReleaseBuffers()
        {
            _vertexBuffer.Release();
        }

        #endregion

        #region Private
        
        private readonly int _vertexBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");
        private ComputeBuffer _vertexBuffer;
        private Material _renderMaterialUnlit;
        private Material _renderMaterialLit;

        #endregion
    }
}