using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for lines. </summary>
    internal class SlamLineCloudRenderer : GpuCloudRenderer<SlamLine, LineCloudBlock, (GpuItem begin, GpuItem end)>
    {
        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public SlamLineCloudRenderer(Shader shader, float scale) : base(shader, scale)
        {
        }
        
        /// <summary> Sets transparency of lines. </summary>
        public void SetAlpha(float alpha)
        {
            foreach (var block in Blocks)
            {
                block.Alpha = alpha;
            }
        }

        #region Protected

        /// <inheritdoc />
        protected override int BlockCapacity => LineCloudBlock.Capacity;

        /// <inheritdoc />
        protected override LineCloudBlock CreateNewBlock() => new LineCloudBlock(Shader, Scale);

        /// <inheritdoc />
        protected override void ProcessItem(LineCloudBlock block, SlamLine item, int inBlockId)
        {
            block[inBlockId] = (new GpuItem(item.Point1), new GpuItem(item.Point2));
        }

        /// <inheritdoc />
        protected override void RemoveItem(LineCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }

        #endregion
    }
}