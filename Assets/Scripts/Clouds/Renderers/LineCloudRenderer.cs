using Elektronik.Data.PackageObjects;

namespace Elektronik.Clouds
{
    public class LineCloudRenderer : CloudRenderer<SlamLine, LineCloudBlock>
    {
        public void SetAlpha(float alpha)
        {
            foreach (var block in Blocks)
            {
                block.Alpha = alpha;
            }
        }
        
        protected override void ProcessItem(LineCloudBlock block, SlamLine item, int inBlockId)
        {
            block.Points[inBlockId * 2 + 0] = new GPUItem(item.Point1);
            block.Points[inBlockId * 2 + 1] = new GPUItem(item.Point2);
        }

        protected override void RemoveItem(LineCloudBlock block, int inBlockId)
        {
            block.Points[inBlockId * 2 + 0] = default;
            block.Points[inBlockId * 2 + 1] = default;
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