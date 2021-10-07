using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MeshRendererBlock : ICloudBlock<GPUItem>
    {
        public float Scale { get; set; } = 1;

        public const int Capacity = 256 * 256 * 3;

        public int RenderQueue => 2000;

        public int ShaderId;

        public MeshRendererBlock(Shader[] shaders)
        {
            _vertices = Enumerable.Repeat(default(GPUItem), Capacity).ToArray();
            UniRxExtensions.StartOnMainThread(() =>
            {
                _materials = shaders.Select(sh => new Material(sh) { hideFlags = HideFlags.DontSave }).ToArray();
                _vertexBuffer = new ComputeBuffer(_vertices.Length, GPUItem.Size);
            }).Subscribe();
        }

        public void UpdateDataOnGpu()
        {
            if (_vertexBuffer is null) return;
            if (_updated) _vertexBuffer.SetData(_vertices);
        }

        public void RenderData()
        {
            if (_materials is null) return;
            var renderMat = _materials[ShaderId % _materials.Length];
            renderMat.SetFloat(_scaleShaderProp, Scale);
            renderMat.SetBuffer(_vertexBufferShaderProp, _vertexBuffer);
            renderMat.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, _vertices.Length);
        }

        public GPUItem this[int index]
        {
            get => _vertices[index];
            set
            {
                _vertices[index] = value;
                _updated = true;
            }
        }

        public void Dispose()
        {
            _vertexBuffer.Release();
        }

        #region Private

        private readonly int _vertexBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");
        private ComputeBuffer _vertexBuffer;
        private Material[] _materials;
        private readonly GPUItem[] _vertices;
        private bool _updated;

        #endregion
    }
}