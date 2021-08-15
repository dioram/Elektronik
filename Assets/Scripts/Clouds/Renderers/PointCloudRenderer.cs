using Elektronik.Data.PackageObjects;

namespace Elektronik.Clouds
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

        public override void SetScale(float value)
        {
            foreach (var block in Blocks)
            {
                block.SetScale(value);
            }
        }
    }
}