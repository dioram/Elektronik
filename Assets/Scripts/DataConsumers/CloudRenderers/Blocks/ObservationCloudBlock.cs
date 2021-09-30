using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudBlock : CloudBlock
    {
        public Matrix4x4[] Transforms;
        public Color[] Colors;

        protected override void Init()
        {
            Transforms = Enumerable.Repeat(default(Matrix4x4), Capacity).ToArray();
            Colors = Enumerable.Repeat(default(Color), Capacity).ToArray();
            _transformsBuffer = new ComputeBuffer(Transforms.Length, sizeof(float) * 16);
            _colorsBuffer = new ComputeBuffer(Colors.Length, sizeof(float) * 4);
        }

        protected override void SendData(Material renderMaterial)
        {
            renderMaterial.SetBuffer(_transformsBufferShaderProp, _transformsBuffer);
            renderMaterial.SetBuffer(_colorsBufferShaderProp, _colorsBuffer);
        }

        protected override void OnUpdated()
        {
            _transformsBuffer.SetData(Transforms);
            _colorsBuffer.SetData(Colors);
        }

        protected override void Draw()
        {
            Graphics.DrawProceduralNow(MeshTopology.Points, _transformsBuffer.count);
        }

        protected override void ReleaseBuffers()
        {
            _transformsBuffer.Release();
            _colorsBuffer.Release();
        }

        #region Private
        
        private readonly int _transformsBufferShaderProp = Shader.PropertyToID("_TransformsBuffer");
        private readonly int _colorsBufferShaderProp = Shader.PropertyToID("_ColorsBuffer");
        private ComputeBuffer _transformsBuffer;
        private ComputeBuffer _colorsBuffer;

        #endregion
    }
}