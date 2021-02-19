using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class PointCloudBlock : CloudBlock
    {
        public GPUItem[] Points;

        public override void Clear()
        {
            ClearArray(Points);
            base.Clear();
        }

        #region Unity events

        private void OnDestroy()
        {
            _pointsBuffer.Release();
        }

        #endregion
        
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

        #endregion

        #region Private definitions
        
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;

        #endregion
    }
}