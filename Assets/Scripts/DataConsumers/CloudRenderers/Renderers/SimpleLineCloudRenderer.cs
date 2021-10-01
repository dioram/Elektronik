using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SimpleLineCloudRenderer : CloudRenderer<SimpleLine, LineCloudBlock, (GPUItem begin, GPUItem end)>
    {
        public void SetAlpha(float alpha)
        {
            foreach (var block in Blocks)
            {
                block.Alpha = alpha;
            }
        }

        protected override int BlockCapacity => LineCloudBlock.Capacity;

        protected override LineCloudBlock CreateNewBlock() => new LineCloudBlock(CloudShader);
        
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