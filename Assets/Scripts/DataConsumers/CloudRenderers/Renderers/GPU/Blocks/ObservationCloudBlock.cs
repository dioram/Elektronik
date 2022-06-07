﻿using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of cloud block for rendering observations. </summary>
    internal class ObservationCloudBlock : CloudBlock<(Matrix4x4 transform, Color color)>
    {
        /// <summary> Amount of observations this cloud block can store. </summary>
        public const int Capacity = 256 * 256;

        /// <inheritdoc />
        public override int RenderQueue => 2000;
        
        /// <summary> Size of observation. </summary>
        public float ItemSize { get; set; }

        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="itemSize"> Initial size of observation. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public ObservationCloudBlock(Shader shader, float itemSize, float scale) : base(scale)
        {
            ItemSize = itemSize;
            _transforms = Enumerable.Repeat(default(Matrix4x4), Capacity).ToArray();
            _colors = Enumerable.Repeat(default(Color), Capacity).ToArray();

            UniRxExtensions.StartOnMainThread(() =>
            {
                RenderMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
                _transformsBuffer = new ComputeBuffer(_transforms.Length, sizeof(float) * 16);
                _colorsBuffer = new ComputeBuffer(_colors.Length, sizeof(float) * 4);
            }).Subscribe();
        }

        public override (Matrix4x4 transform, Color color) this[int index]
        {
            get => (_transforms[index], _colors[index]);
            set
            {
                _transforms[index] = value.transform;
                _colors[index] = value.color;
                Updated = true;
            }
        }

        /// <inheritdoc />
        public override void UpdateDataOnGpu()
        {
            if (_colorsBuffer is null) return;
            if (Updated)
            {
                _transformsBuffer.SetData(_transforms);
                _colorsBuffer.SetData(_colors);
            }

            base.UpdateDataOnGpu();
        }

        /// <inheritdoc />
        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            RenderMaterial.SetBuffer(_transformsBufferShaderProp, _transformsBuffer);
            RenderMaterial.SetBuffer(_colorsBufferShaderProp, _colorsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, _transformsBuffer.count);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            _transformsBuffer.Release();
            _colorsBuffer.Release();
        }

        #region Private

        private readonly int _transformsBufferShaderProp = Shader.PropertyToID("_TransformsBuffer");
        private readonly int _colorsBufferShaderProp = Shader.PropertyToID("_ColorsBuffer");
        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private ComputeBuffer _transformsBuffer;
        private ComputeBuffer _colorsBuffer;
        private readonly Matrix4x4[] _transforms;
        private readonly Color[] _colors;

        #endregion
    }
}