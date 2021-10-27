using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of cloud block for rendering meshes. </summary>
    internal class MeshRendererBlock : ICloudBlock<GpuItem>
    {
        /// <inheritdoc />
        public float Scale { get; set; }

        /// <summary> Amount of vertices this cloud block can store. </summary>
        /// <remarks> Note that triangles can't share vertices. </remarks>
        public const int Capacity = 256 * 256 * 3;

        /// <inheritdoc />
        public int RenderQueue => 2000;

        /// <summary> Id of shader that should be used for rendering. </summary>
        public int ShaderId;

        /// <summary> Constructor. </summary>
        /// <param name="shaders"> Shaders for rendering this block. They should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public MeshRendererBlock(Shader[] shaders, float scale)
        {
            Scale = scale;
            _vertices = Enumerable.Repeat(default(GpuItem), Capacity).ToArray();
            UniRxExtensions.StartOnMainThread(() =>
            {
                _materials = shaders.Select(sh => new Material(sh) { hideFlags = HideFlags.DontSave }).ToArray();
                _vertexBuffer = new ComputeBuffer(_vertices.Length, GpuItem.Size);
            }).Subscribe();
        }

        /// <inheritdoc />
        public void UpdateDataOnGpu()
        {
            if (_vertexBuffer is null || !_updated) return;
            _vertexBuffer.SetData(_vertices);
            _updated = false;
        }

        /// <inheritdoc />
        public void RenderData()
        {
            if (_materials is null) return;
            var renderMat = _materials[ShaderId % _materials.Length];
            renderMat.SetFloat(_scaleShaderProp, Scale);
            renderMat.SetBuffer(_vertexBufferShaderProp, _vertexBuffer);
            renderMat.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, Capacity);
        }

        public GpuItem this[int index]
        {
            get => _vertices[index];
            set
            {
                _vertices[index] = value;
                _updated = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _vertexBuffer.Release();
        }

        #region Private

        private readonly int _vertexBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");
        private ComputeBuffer _vertexBuffer;
        private Material[] _materials;
        private readonly GpuItem[] _vertices;
        private bool _updated;

        #endregion
    }
}