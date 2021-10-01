using System.Linq;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MarkerCloudBlock : CloudBlock<MarkerCloudBlock.MarkerGpuData>
    {
        public struct MarkerGpuData
        {
            public Matrix4x4 Transform;
            public Vector3 Scale;
            public Color Color;

            public MarkerGpuData(Matrix4x4 transform, Vector3 scale, Color color)
            {
                Transform = transform;
                Scale = scale;
                Color = color;
            }
        }

        public const int Capacity = 256 * 256;

        public override int RenderQueue => 2000;

        public MarkerCloudBlock(Shader shader)
        {
            _transforms = Enumerable.Repeat(default(Matrix4x4), Capacity).ToArray();
            _scales = Enumerable.Repeat(default(Vector3), Capacity).ToArray();
            _colors = Enumerable.Repeat(default(Color), Capacity).ToArray();
            MainThreadInvoker.Enqueue(() =>
            {
                RenderMaterial = new Material(shader) {hideFlags = HideFlags.DontSave};
                _transformsBuffer = new ComputeBuffer(Capacity, sizeof(float) * 16);
                _scalesBuffer = new ComputeBuffer(Capacity, sizeof(float) * 3);
                _colorsBuffer = new ComputeBuffer(Capacity, sizeof(float) * 4);
                _initialized = true;
            });
        }

        public override void UpdateDataOnGPU()
        {
            if (!_initialized) return;
            if (Updated)
            {
                _transformsBuffer.SetData(_transforms);
                _scalesBuffer.SetData(_scales);
                _colorsBuffer.SetData(_colors);
            }
            base.UpdateDataOnGPU();
        }

        public override void RenderData()
        {
            base.RenderData();
            RenderMaterial.SetBuffer(_transformsBufferShaderProp, _transformsBuffer);
            RenderMaterial.SetBuffer(_scalesBufferShaderProp, _scalesBuffer);
            RenderMaterial.SetBuffer(_colorsBufferShaderProp, _colorsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, Capacity);
        }

        public override MarkerGpuData this[int index]
        {
            get => new MarkerGpuData(_transforms[index], _scales[index], _colors[index]);
            set
            {
                _transforms[index] = value.Transform;
                _scales[index] = value.Scale;
                _colors[index] = value.Color;
                Updated = true;
            }
        }

        public override void Dispose()
        {
            _transformsBuffer.Release();
            _scalesBuffer.Release();
            _colorsBuffer.Release();
        }

        #region Private

        private readonly int _transformsBufferShaderProp = Shader.PropertyToID("_TransformsBuffer");
        private readonly int _scalesBufferShaderProp = Shader.PropertyToID("_ScalesBuffer");
        private readonly int _colorsBufferShaderProp = Shader.PropertyToID("_ColorsBuffer");
        private ComputeBuffer _transformsBuffer;
        private ComputeBuffer _scalesBuffer;
        private ComputeBuffer _colorsBuffer;
        private readonly Matrix4x4[] _transforms;
        private readonly Vector3[] _scales;
        private readonly Color[] _colors;
        private bool _initialized;

        #endregion
    }
}