using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PlaneCloudRenderer : CloudRenderer<SlamPlane, PlaneCloudBlock, GPUItem[]>, IResizableRenderer
    {
        public PlaneCloudRenderer(Shader shader) : base(shader)
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

        protected override int BlockCapacity => PlaneCloudBlock.Capacity;

        protected override PlaneCloudBlock CreateNewBlock() => new PlaneCloudBlock(Shader, ItemSize);

        protected override void ProcessItem(PlaneCloudBlock block, SlamPlane item, int inBlockId)
        {
            var halfSide = ItemSize / 2;
            var v1 = new Vector3(-halfSide, 0, -halfSide);
            var v2 = new Vector3(halfSide, 0, -halfSide);
            var v3 = new Vector3(halfSide, 0, halfSide);
            var v4 = new Vector3(-halfSide, 0, halfSide);

            var rotation = Quaternion.FromToRotation(Vector3.up, item.Normal);

            var vertices = new[]
            {
                new GPUItem(rotation * v1 + item.Offset, item.Color),
                new GPUItem(rotation * v2 + item.Offset, item.Color),
                new GPUItem(rotation * v3 + item.Offset, item.Color),
                new GPUItem(rotation * v4 + item.Offset, item.Color),
            };
            block[inBlockId] = vertices;
        }

        protected override void RemoveItem(PlaneCloudBlock block, int inBlockId)
        {
            block[inBlockId] = _defaultData;
        }

        private readonly GPUItem[] _defaultData =
                Enumerable.Repeat<GPUItem>(default, PlaneCloudBlock.VerticesPerPlane).ToArray();

        private float _itemSize;
    }
}