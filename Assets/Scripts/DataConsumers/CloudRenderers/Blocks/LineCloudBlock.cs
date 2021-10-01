﻿using System.Linq;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class LineCloudBlock : CloudBlock<(GPUItem begin, GPUItem end)>
    {
        public const int VerticesPerLine = 2;

        public const int Capacity = 256 * 256;

        public override int RenderQueue => 2000;

        public float Alpha = 1;

        public LineCloudBlock(Shader shader)
        {
            _points = Enumerable.Repeat(default(GPUItem), Capacity * VerticesPerLine).ToArray();
            MainThreadInvoker.Enqueue(() =>
            {
                RenderMaterial = new Material(shader) {hideFlags = HideFlags.DontSave};
                _pointsBuffer = new ComputeBuffer(_points.Length, GPUItem.Size);
                _initialized = true;
            });
        }

        public override void UpdateDataOnGPU()
        {
            if (!_initialized) return;
            if (Updated) _pointsBuffer.SetData(_points);
            base.UpdateDataOnGPU();
        }

        public override void RenderData()
        {
            base.RenderData();
            RenderMaterial.SetFloat(_alphaShaderProp, Alpha);
            RenderMaterial.SetBuffer(_pointsBufferShaderProp, _pointsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Lines, _pointsBuffer.count);
        }

        public override (GPUItem begin, GPUItem end) this[int index]
        {
            get => (_points[index * VerticesPerLine], _points[index * VerticesPerLine + 1]);
            set
            {
                _points[index * VerticesPerLine] = value.begin;
                _points[index * VerticesPerLine + 1] = value.end;
                Updated = true;
            }
        }

        public override void Dispose()
        {
            _pointsBuffer.Release();
        }

        #region Private definitions

        private readonly int _alphaShaderProp = Shader.PropertyToID("_Alpha");
        private readonly int _pointsBufferShaderProp = Shader.PropertyToID("_ItemsBuffer");
        private ComputeBuffer _pointsBuffer;
        private readonly GPUItem[] _points;
        private bool _initialized;

        #endregion
    }
}