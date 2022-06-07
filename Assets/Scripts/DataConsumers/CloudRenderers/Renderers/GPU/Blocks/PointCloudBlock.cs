using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of cloud block for rendering points. </summary>
    internal class PointCloudBlock : CloudBlock<GpuItem>
    {
        /// <summary> Size of point. </summary>
        public float ItemSize { get; set; }

        /// <inheritdoc />
        public override int RenderQueue => 2000;

        /// <summary> Amount of points this cloud block can store. </summary>
        public const int Capacity = 256 * 256;

        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="itemSize"> Initial size of observation. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public PointCloudBlock(Shader shader, float itemSize, float scale) : base(scale)
        {
            ItemSize = itemSize;
            _points = Enumerable.Repeat(default(GpuItem), Capacity).ToArray();
            UniRxExtensions.StartOnMainThread(() =>
            {
                RenderMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
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
            RenderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            RenderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, _pointsBuffer.count);
        }

        public override GpuItem this[int index]
        {
            get => _points[index];
            set
            {
                _points[index] = value;
                Updated = true;
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _pointsBuffer.Release();
        }

        #region Private

        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;
        private readonly GpuItem[] _points;

        #endregion
    }
}