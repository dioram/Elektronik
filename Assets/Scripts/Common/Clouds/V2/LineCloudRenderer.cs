using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Clouds.V2
{
    public class LineCloudRenderer : CloudRenderer<SlamLine, LineCloudBlock>
    {
        protected override void ProcessItem(LineCloudBlock block, SlamLine item)
        {
            var inBlockId = (item.Id % LineCloudBlock.Capacity);
            block.Points[inBlockId * 2 + 0] = new GPUItem(item.pt1);
            block.Points[inBlockId * 2 + 1] = new GPUItem(item.pt2);
        }

        protected override void RemoveItem(LineCloudBlock block, int id)
        {
            var inBlockId = (id % LineCloudBlock.Capacity);
            block.Points[inBlockId * 2 + 0] = default;
            block.Points[inBlockId * 2 + 1] = default;
        }
    }
}