using System;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Clouds
{
    public class SimpleLineCloudRenderer : CloudRenderer<SimpleLine, LineCloudBlock>
    {
        public void SetAlpha(float alpha)
        {
            foreach (var block in Blocks)
            {
                block.Alpha = alpha;
            }
        }
        
        protected override void ProcessItem(LineCloudBlock block, SimpleLine item, int inBlockId)
        {
            block.Points[inBlockId * 2 + 0] = new GPUItem(item.BeginPos, item.BeginColor);
            block.Points[inBlockId * 2 + 1] = new GPUItem(item.EndPos, item.EndColor);
        }

        protected override void RemoveItem(LineCloudBlock block, int inBlockId)
        {
            block.Points[inBlockId * 2 + 0] = default;
            block.Points[inBlockId * 2 + 1] = default;
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