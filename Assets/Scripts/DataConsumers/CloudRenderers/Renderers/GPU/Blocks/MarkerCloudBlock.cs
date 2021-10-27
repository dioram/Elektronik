using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of cloud block for rendering lines. </summary>
    internal class MarkerCloudBlock : CloudBlock<MarkerCloudBlock.MarkerGpuData>
    {
        /// <summary> Struct for storing data that will be sent to GPU. </summary>
        internal struct MarkerGpuData
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

        /// <summary> Amount of markers this cloud block can store. </summary>
        public const int Capacity = 256 * 256;

        /// <inheritdoc />
        public override int RenderQueue => 2000;

        /// <summary> Constructor. </summary>
        public MarkerCloudBlock() : base(1)
        {
            _transforms = Enumerable.Repeat(default(Matrix4x4), Capacity).ToArray();
            _scales = Enumerable.Repeat(default(Vector3), Capacity).ToArray();
            _colors = Enumerable.Repeat(default(Color), Capacity).ToArray();
        }

        /// <summary> Sets shader. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        public void InitShader(Shader shader)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                RenderMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
                _transformsBuffer = new ComputeBuffer(Capacity, sizeof(float) * 16);
                _scalesBuffer = new ComputeBuffer(Capacity, sizeof(float) * 3);
                _colorsBuffer = new ComputeBuffer(Capacity, sizeof(float) * 4);
            }).Subscribe();
        }

        /// <inheritdoc />
        public override void UpdateDataOnGpu()
        {
            if (_colorsBuffer is null) return;
            if (Updated)
            {
                _transformsBuffer.SetData(_transforms);
                _scalesBuffer.SetData(_scales);
                _colorsBuffer.SetData(_colors);
            }

            base.UpdateDataOnGpu();
        }

        /// <inheritdoc />
        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
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

        /// <inheritdoc />
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

        #endregion
    }
}