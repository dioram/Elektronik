using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for observation. </summary>
    internal class ObservationCloudRenderer 
            : GpuCloudRenderer<SlamObservation, ObservationCloudBlock, (Matrix4x4 transform, Color color)>,
              IResizableRenderer
    {
        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        /// <param name="itemSize"> Initial size of observation. </param>
        public ObservationCloudRenderer(Shader shader, float scale, float itemSize) : base(shader, scale)
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

        #region MyRegion

        /// <inheritdoc />
        protected override void ProcessItem(ObservationCloudBlock block, SlamObservation item, int inBlockId)
        {
            block[inBlockId] = (Matrix4x4.TRS(item.Point.Position, item.Rotation, Vector3.one), item.Point.Color);
        }

        /// <inheritdoc />
        protected override void RemoveItem(ObservationCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }

        /// <inheritdoc />
        protected override int BlockCapacity => ObservationCloudBlock.Capacity;

        /// <inheritdoc />
        protected override ObservationCloudBlock CreateNewBlock() => new ObservationCloudBlock(Shader, ItemSize, Scale);

        #endregion

        #region Private

        private float _itemSize;

        #endregion
        
    }
}