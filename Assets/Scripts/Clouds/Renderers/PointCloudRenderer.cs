using System;
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

        public override float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;
                
                foreach (var block in Blocks)
                {
                    block.SetScale(value);
                } 
            }
        }

        private float _scale;
    }
}