using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of cloud block for rendering planes. </summary>
    internal class PlaneCloudBlock : CloudBlock<GpuItem[]>
    {
        /// <summary> Edge size of plane. </summary>
        public float ItemSize { get; set; }

        /// <summary> Amount of planes this cloud block can store. </summary>
        public const int Capacity = 256 * 256;

        /// <inheritdoc />
        public override int RenderQueue => 2000;

        public const int VerticesPerPlane = 4;

        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="itemSize"> Initial size of observation. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public PlaneCloudBlock(Shader shader, float itemSize, float scale) : base(scale)
        {
            ItemSize = itemSize;
            _planes = Enumerable.Repeat(default(GpuItem), Capacity * VerticesPerPlane).ToArray();
            UniRxExtensions.StartOnMainThread(() =>
            {
                RenderMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
                _vertsBuffer = new ComputeBuffer(_planes.Length, GpuItem.Size);
            }).Subscribe();
        }

        /// <inheritdoc />
        public override void UpdateDataOnGpu()
        {
            if (_vertsBuffer is null) return;
            if (Updated) _vertsBuffer.SetData(_planes);
            base.UpdateDataOnGpu();
        }

        /// <inheritdoc />
        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            RenderMaterial.SetBuffer(_vertsBufferShaderProp, _vertsBuffer);
            RenderMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Quads, _vertsBuffer.count);
        }

        public override GpuItem[] this[int index]
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

        /// <inheritdoc />
        public override void Dispose()
        {
            _vertsBuffer.Release();
        }

        #region Private

        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private readonly int _vertsBufferShaderProp = Shader.PropertyToID("_VertsBuffer");
        private ComputeBuffer _vertsBuffer;
        private readonly GpuItem[] _planes;

        #endregion
    }
}