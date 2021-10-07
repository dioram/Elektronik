using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PlaneCloudBlock : CloudBlock<GPUItem[]>
    {
        public float ItemSize { get; set; }
        public const int Capacity = 256 * 256;
        public override int RenderQueue => 2000;

        public const int VerticesPerPlane = 4;

        public PlaneCloudBlock(Shader shader, float itemSize, float scale) : base(scale)
        {
            ItemSize = itemSize;
            _planes = Enumerable.Repeat(default(GPUItem), Capacity * VerticesPerPlane).ToArray();
            UniRxExtensions.StartOnMainThread(() =>
            {
                RenderMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
                _vertsBuffer = new ComputeBuffer(_planes.Length, GPUItem.Size);
            }).Subscribe();
        }

        public override void UpdateDataOnGpu()
        {
            if (_vertsBuffer is null) return;
            if (Updated) _vertsBuffer.SetData(_planes);
            base.UpdateDataOnGpu();
        }

        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            RenderMaterial.SetBuffer(_vertsBufferShaderProp, _vertsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Quads, _vertsBuffer.count);
        }

        public override GPUItem[] this[int index]
        {
            get => _planes.Skip(index).Take(VerticesPerPlane).ToArray();
            set
            {
                if (value.Length != VerticesPerPlane)
                {
                    throw new ArgumentException("Wrong amount of vertices for plane");
                }

                for (var i = 0; i < VerticesPerPlane; i++)
                {
                    _planes[index * VerticesPerPlane + i] = value[i];
                }

                Updated = true;
            }
        }

        public override void Dispose()
        {
            _vertsBuffer.Release();
        }

        #region Private definitions

        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private readonly int _vertsBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private ComputeBuffer _vertsBuffer;
        private readonly GPUItem[] _planes;

        #endregion
    }
}