using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for points. </summary>
    internal class PointCloudRenderer : GpuCloudRenderer<SlamPoint, PointCloudBlock, GpuItem>, IResizableRenderer
    {
        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        /// <param name="itemSize"> Initial size of point edge. </param>
        public PointCloudRenderer(Shader shader, float scale, float itemSize) : base(shader, scale)
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
        protected override int BlockCapacity => PointCloudBlock.Capacity;

        /// <inheritdoc />
        protected override PointCloudBlock CreateNewBlock() => new PointCloudBlock(Shader, ItemSize, Scale);

        /// <inheritdoc />
        protected override void ProcessItem(PointCloudBlock block, SlamPoint item, int inBlockId)
        {
            block[inBlockId] = new GpuItem(item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(PointCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }

        #endregion

        #region Private

        private float _itemSize;

        #endregion
    }
}