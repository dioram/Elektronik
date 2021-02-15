namespace Elektronik.Common.Clouds.V2
{
    public class PointCloudRenderer : CloudRenderer<CloudPoint, PointCloudBlock>
    {
        protected override void ProcessItem(PointCloudBlock block, CloudPoint item)
        {
            block.Points[item.Id % PointCloudBlock.Capacity] = new GPUItem(item);
        }
    }
}