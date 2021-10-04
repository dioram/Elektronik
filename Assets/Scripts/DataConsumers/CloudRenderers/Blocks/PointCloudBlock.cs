using System.Linq;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PointCloudBlock : CloudBlock<GPUItem>
    {
        public float ItemSize { get; set; }
        public override int RenderQueue => 2000;
        public const int Capacity = 256 * 256;
        
        public PointCloudBlock(Shader shader, float itemSize)
        {
            ItemSize = itemSize;
            _points = Enumerable.Repeat(default(GPUItem), Capacity).ToArray();
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                RenderMaterial = new Material(shader) {hideFlags = HideFlags.DontSave};
                _pointsBuffer = new ComputeBuffer(_points.Length, GPUItem.Size);
                _initialized = true;
            });
        }

        public override void UpdateDataOnGpu()
        {
            if (!_initialized) return;
            if (Updated) _pointsBuffer.SetData(_points);
            base.UpdateDataOnGpu();
        }

        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            RenderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, _pointsBuffer.count);
        }

        public override GPUItem this[int index]
        {
            get => _points[index];
            set
            {
                _points[index] = value;
                Updated = true;
            }
        }

        public override void Dispose()
        {
            _pointsBuffer.Release();
        }

        #region Private

        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;
        private readonly GPUItem[] _points;
        private bool _initialized;

        #endregion
    }
}