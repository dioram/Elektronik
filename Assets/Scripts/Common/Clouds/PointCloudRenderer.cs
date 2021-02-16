using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Clouds
{
    public class PointCloudRenderer : CloudRenderer<SlamPoint, PointCloudBlock>
    {
        protected override void ProcessItem(PointCloudBlock block, SlamPoint item, int inBlockId)
        {
            block.Points[inBlockId] = new GPUItem(item);
        }

        protected override void RemoveItem(PointCloudBlock block, int inBlockId)
        {
            block.Points[inBlockId] = default;
        }
    }
}