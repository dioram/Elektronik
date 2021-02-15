namespace Elektronik.Common.Clouds.V2
{
    public class LineCloudRenderer : CloudRenderer<CloudLine, LineCloudBlock>
    {
        protected override void ProcessItem(LineCloudBlock block, CloudLine item)
        {
            block.Points[item.Id % LineCloudBlock.Capacity * 2 + 0] = new GPUItem(item.pt1);
            block.Points[item.Id % LineCloudBlock.Capacity * 2 + 1] = new GPUItem(item.pt2);
        }
    }
}