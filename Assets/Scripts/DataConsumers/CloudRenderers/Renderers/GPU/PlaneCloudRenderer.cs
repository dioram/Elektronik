using System.Linq;
using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for planes. </summary>
    internal class PlaneCloudRenderer : GpuCloudRenderer<SlamPlane, PlaneCloudBlock, GpuItem[]>, IResizableRenderer
    {
        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        /// <param name="itemSize"> Initial size of plane edge. </param>
        public PlaneCloudRenderer(Shader shader, float scale, float itemSize) : base(shader, scale)
        {
            _itemSize = itemSize;
        }

        #region IResizableRenderer

        /// <inheritdoc />
        public float ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                foreach (var block in Blocks)
                {
                    block.ItemSize = ItemSize;
                }
            }
        }

        #endregion

        #region Protected

        /// <inheritdoc />
        protected override int BlockCapacity => PlaneCloudBlock.Capacity;

        /// <inheritdoc />
        protected override PlaneCloudBlock CreateNewBlock() => new PlaneCloudBlock(Shader, ItemSize, Scale);

        /// <inheritdoc />
        protected override void ProcessItem(PlaneCloudBlock block, SlamPlane item, int inBlockId)
        {
            var halfSide = ItemSize / 2;
            var v1 = new Vector3(-halfSide, 0, -halfSide);
            var v2 = new Vector3(halfSide, 0, -halfSide);
            var v3 = new Vector3(halfSide, 0, halfSide);
            var v4 = new Vector3(-halfSide, 0, halfSide);

            var rotation = Quaternion.FromToRotation(Vector3.up, item.Normal);

            var vertices = new[]
            {
                new GpuItem(rotation * v1 + item.Offset, item.Color),
                new GpuItem(rotation * v2 + item.Offset, item.Color),
                new GpuItem(rotation * v3 + item.Offset, item.Color),
                new GpuItem(rotation * v4 + item.Offset, item.Color),
            };
            block[inBlockId] = vertices;
        }

        /// <inheritdoc />
        protected override void RemoveItem(PlaneCloudBlock block, int inBlockId)
        {
            block[inBlockId] = DefaultData;
        }

        #endregion

        #region Private

        private static readonly GpuItem[] DefaultData =
                Enumerable.Repeat<GpuItem>(default, PlaneCloudBlock.VerticesPerPlane).ToArray();

        private float _itemSize;

        #endregion
    }
}