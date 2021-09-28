using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MeshRendererBlock : CloudBlock
    {
        public GPUItem[] Vertices;
        public Shader[] Shaders;
        public int ShaderId;
        
        public new const int Capacity = 256 * 256 * 3;

        public override GPUItem[] GetItems() => Vertices;

        public override void SetScale(float value)
        {
            foreach (var material in _materials)
            {
                material.SetFloat(_scaleShaderProp, value);
            }
        }

        #region Unity events

        protected override void Start()
        {
            _materials = Shaders.Select(sh => new Material(sh) { hideFlags = HideFlags.DontSave }).ToArray();
            foreach (var material in _materials)
            {
                material.EnableKeyword("_COMPUTE_BUFFER");
            }
        }
        
        protected override void OnRenderObject()
        {
            var renderMat = _materials[ShaderId % _materials.Length];
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
        private Material[] _materials;

        #endregion
    }
}