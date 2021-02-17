using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class InfinitePlaneCloudBlock : CloudBlock
    {
        public GPUItem[] Planes;

        public override void Clear()
        {
            ClearArray(Planes);
            base.Clear();
        }

        #region Unity events

        private void OnDestroy()
        {
            _vertsBuffer.Release();
        }

        #endregion

        #region Protected definitions

        protected override void Init()
        {
            Planes = Enumerable.Repeat(default(GPUItem), Capacity * 8).ToArray();
            _vertsBuffer = new ComputeBuffer(Planes.Length, GPUItem.Size);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetBuffer(_vertsBufferShaderProp, _vertsBuffer);
        }

        protected override void OnUpdated()
        {
            _vertsBuffer.SetData(Planes);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Quads, _vertsBuffer.count);
        }

        #endregion

        #region Private definitions

        private readonly int _vertsBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private ComputeBuffer _vertsBuffer;

        #endregion
    }
}