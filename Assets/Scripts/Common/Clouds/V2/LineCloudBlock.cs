using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds.V2
{
    public class LineCloudBlock : CloudBlock
    {
        public GPUItem[] Points;

        protected override void Init()
        {
            Points = Enumerable.Repeat(default(GPUItem), Capacity * 2).ToArray();
            _pointsBuffer = new ComputeBuffer(Points.Length, GPUItem.Size);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
        }

        protected override void Clear()
        {
        }

        protected override void OnUpdated()
        {
            _pointsBuffer.SetData(Points);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Lines, _pointsBuffer.count, 1);
        }

        #region Private definitions

        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;

        #endregion
    }
}