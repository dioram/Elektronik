using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of cloud block for rendering lines. </summary>
    internal class LineCloudBlock : CloudBlock<(GpuItem begin, GpuItem end)>
    {
        public const int VerticesPerLine = 2;

        /// <summary> Amount of lines this cloud block can store. </summary>
        public const int Capacity = 256 * 256;

        /// <inheritdoc />
        public override int RenderQueue => 2000;

        /// <summary> Transparency of lines. </summary>
        public float Alpha = 1;

        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public LineCloudBlock(Shader shader, float scale) : base(scale)
        {
            _points = Enumerable.Repeat(default(GpuItem), Capacity * VerticesPerLine).ToArray();
            UniRxExtensions.StartOnMainThread(() =>
            {
                RenderMaterial = new Material(shader) {hideFlags = HideFlags.DontSave};
                _pointsBuffer = new ComputeBuffer(_points.Length, GpuItem.Size);
            }).Subscribe();
        }

        /// <inheritdoc />
        public override void UpdateDataOnGpu()
        {
            if (_pointsBuffer is null) return;
            if (Updated) _pointsBuffer.SetData(_points);
            base.UpdateDataOnGpu();
        }

        /// <inheritdoc />
        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_alphaShaderProp, Alpha);
            RenderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Lines, _pointsBuffer.count);
        }

        public override (GpuItem begin, GpuItem end) this[int index]
        {
            get => (_points[index * VerticesPerLine], _points[index * VerticesPerLine + 1]);
            set
            {
                _points[index * VerticesPerLine] = value.begin;
                _points[index * VerticesPerLine + 1] = value.end;
                Updated = true;
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _pointsBuffer.Release();
        }

        #region Private definitions

        private readonly int _alphaShaderProp = Shader.PropertyToID("_Alpha");
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;
        private readonly GpuItem[] _points;

        #endregion
    }
}