using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Clouds
{
    public class LineCloudRenderer : CloudRenderer<SlamLine, LineCloudBlock>
    {
        public static LineCloudRenderer StaticRenderer;

        #region Unity events

        private void Awake()
        {
            StaticRenderer = this;
        }

        #endregion
        
        protected override void ProcessItem(LineCloudBlock block, SlamLine item, int inBlockId)
        {
            block.Points[inBlockId * 2 + 0] = new GPUItem(item.pt1);
            block.Points[inBlockId * 2 + 1] = new GPUItem(item.pt2);
        }

        protected override void RemoveItem(LineCloudBlock block, int inBlockId)
        {
            block.Points[inBlockId * 2 + 0] = default;
            block.Points[inBlockId * 2 + 1] = default;
        }
    }
}