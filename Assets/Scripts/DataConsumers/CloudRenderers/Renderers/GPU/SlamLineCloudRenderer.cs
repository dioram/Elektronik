using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SlamLineCloudRenderer : GpuCloudRenderer<SlamLine, LineCloudBlock, (GPUItem begin, GPUItem end)>
    {
        public SlamLineCloudRenderer(Shader shader) : base(shader)
        {
        }
        
        public void SetAlpha(float alpha)
        {
            foreach (var block in Blocks)
            {
                block.Alpha = alpha;
            }
        }

        protected override int BlockCapacity => LineCloudBlock.Capacity;

        protected override LineCloudBlock CreateNewBlock() => new LineCloudBlock(Shader);
        
        protected override void ProcessItem(LineCloudBlock block, SlamLine item, int inBlockId)
        {
            block[inBlockId] = (new GPUItem(item.Point1), new GPUItem(item.Point2));
        }

        protected override void RemoveItem(LineCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }
    }
}