using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PointCloudRenderer : CloudRenderer<SlamPoint, PointCloudBlock, GPUItem>, IResizableRenderer
    {

        public PointCloudRenderer(Shader shader) : base(shader)
        {
        }

        public float ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                foreach (var block in Blocks)
                {
                    block.ItemSize = ItemSize;
                }
            }
        }

        protected override int BlockCapacity => PointCloudBlock.Capacity;
        
        protected override PointCloudBlock CreateNewBlock() => new PointCloudBlock(Shader, ItemSize);
        
        protected override void ProcessItem(PointCloudBlock block, SlamPoint item, int inBlockId)
        {
            block[inBlockId] = new GPUItem(item);
        }

        protected override void RemoveItem(PointCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }
        
        private float _itemSize;
    }
}