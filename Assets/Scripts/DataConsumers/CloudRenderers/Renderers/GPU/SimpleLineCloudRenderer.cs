using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SimpleLineCloudRenderer : GpuCloudRenderer<SimpleLine, LineCloudBlock, (GPUItem begin, GPUItem end)>
    {
        public SimpleLineCloudRenderer(Shader shader, float scale) : base(shader, scale)
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

        protected override LineCloudBlock CreateNewBlock() => new LineCloudBlock(Shader, Scale);
        
        protected override void ProcessItem(LineCloudBlock block, SimpleLine item, int inBlockId)
        {
            block[inBlockId] = (new GPUItem(item.BeginPos, item.BeginColor), new GPUItem(item.EndPos, item.EndColor));
        }

        protected override void RemoveItem(LineCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }
    }
}