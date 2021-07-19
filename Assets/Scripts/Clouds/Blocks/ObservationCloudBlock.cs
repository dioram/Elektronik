using System.Linq;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class ObservationCloudBlock : CloudBlock
    {
        public GPUItem[] Points;
        public const int CapacityMultiplier = 18;

        public override GPUItem[] GetItems() => Points;

        public override void Clear()
        {
            ClearArray(Points);
            base.Clear();
        }
        
        protected override void Init()
        {
            Points = Enumerable.Repeat(default(GPUItem), Capacity * CapacityMultiplier).ToArray();
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
            Graphics.DrawProceduralNow(MeshTopology.Triangles, _pointsBuffer.count);
        }

        #region Private
        
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;

        #endregion
    }
}