using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PointCloudBlock : CloudBlock
    {
        public GPUItem[] Points;

        #region Protected definitions

        protected override void Init()
        {
            Points = Enumerable.Repeat(default(GPUItem), Capacity).ToArray();
            _pointsBuffer = new ComputeBuffer(Points.Length, GPUItem.Size);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
        }
        
        protected override void OnUpdated()
        {
            _pointsBuffer.SetData(Points);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Points, _pointsBuffer.count);
        }

        protected override void ReleaseBuffers()
        {
            _pointsBuffer.Release();
        }

        #endregion

        #region Private definitions
        
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;

        #endregion
    }
}