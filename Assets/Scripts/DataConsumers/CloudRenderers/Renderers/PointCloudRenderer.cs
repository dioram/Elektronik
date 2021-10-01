using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PointCloudRenderer : CloudRenderer<SlamPoint, PointCloudBlock, GPUItem>, IResizableRenderer
    {
        public float ItemSize;
        
        public void SetSize(float newSize)
        {
            ItemSize = newSize;

            foreach (var block in Blocks)
            {
                block.ItemSize = ItemSize;
            }
        }

        protected override int BlockCapacity => PointCloudBlock.Capacity;
        
        protected override PointCloudBlock CreateNewBlock() => new PointCloudBlock(CloudShader, ItemSize);
        
        protected override void ProcessItem(PointCloudBlock block, SlamPoint item, int inBlockId)
        {
            block[inBlockId] = new GPUItem(item);
        }

        protected override void RemoveItem(PointCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }
    }
}