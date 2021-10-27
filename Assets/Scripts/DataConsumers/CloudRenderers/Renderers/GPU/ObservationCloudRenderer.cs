using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudRenderer 
            : GpuCloudRenderer<SlamObservation, ObservationCloudBlock, (Matrix4x4 transform, Color color)>,
              IResizableRenderer
    {
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
        
        public ObservationCloudRenderer(Shader shader, float scale, float itemSize) : base(shader, scale)
        {
            _itemSize = itemSize;
        }
        
        protected override void ProcessItem(ObservationCloudBlock block, SlamObservation item, int inBlockId)
        {
            block[inBlockId] = (Matrix4x4.TRS(item.Position, item.Rotation, Vector3.one), item.Color);
        }

        protected override void RemoveItem(ObservationCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }

        protected override int BlockCapacity => ObservationCloudBlock.Capacity;
        
        protected override ObservationCloudBlock CreateNewBlock() => new ObservationCloudBlock(Shader, ItemSize, Scale);
        
        private float _itemSize;
    }
}