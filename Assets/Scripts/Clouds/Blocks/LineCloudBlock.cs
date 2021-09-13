using System.Linq;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class LineCloudBlock : CloudBlock
    {
        public GPUItem[] Points;

        [Range(0, 1)] public float Alpha = 1;

        public override GPUItem[] GetItems() => Points;

        public override void Clear()
        {
            ClearArray(Points);
            base.Clear();
        }

        #region Protected

        protected override void Init()
        {
            Points = Enumerable.Repeat(default(GPUItem), Capacity * 2).ToArray();
            _pointsBuffer = new ComputeBuffer(Points.Length, GPUItem.Size);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetFloat(_alphaShaderProp, Alpha);
            renderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
        }

        protected override void OnUpdated()
        {
            _pointsBuffer.SetData(Points);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Lines, _pointsBuffer.count);
        }
        
        protected override void ReleaseBuffers()
        {
            _pointsBuffer.Release();
        }

        #endregion

        #region Private definitions

        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;
        private readonly int _alphaShaderProp = Shader.PropertyToID("_Alpha");

        #endregion
    }
}