using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Clouds.V2
{
    public class PointCloudRenderer : CloudRenderer<SlamPoint, PointCloudBlock>
    {
        protected override void ProcessItem(PointCloudBlock block, SlamPoint item)
        {
            var inBlockId = (item.Id % PointCloudBlock.Capacity);
            block.Points[inBlockId] = new GPUItem(item);
        }

        protected override void RemoveItem(PointCloudBlock block, int id)
        {
            var inBlockId = (id % PointCloudBlock.Capacity);
            block.Points[inBlockId] = default;
        }
    }
}