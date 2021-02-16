using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Clouds.V2
{
    public class InfinitePlaneCloudRenderer : CloudRenderer<SlamInfinitePlane, InfinitePlaneCloudBlock>
    {
        protected override void ProcessItem(InfinitePlaneCloudBlock block, SlamInfinitePlane item)
        {
            var inBlockId = (item.Id % InfinitePlaneCloudBlock.Capacity);
            float halfSide = ItemSize / 2;
            var v1 = new Vector3(-halfSide, 0, -halfSide);
            var v2 = new Vector3(halfSide, 0, -halfSide);
            var v3 = new Vector3(halfSide, 0, halfSide);
            var v4 = new Vector3(-halfSide, 0, halfSide);

            var rotation = Quaternion.FromToRotation(Vector3.up, item.normal);
            
            var vertices = new []
            {
                    rotation * v1 + item.offset,
                    rotation * v2 + item.offset,
                    rotation * v3 + item.offset,
                    rotation * v4 + item.offset,
                    rotation * v4 + item.offset,
                    rotation * v3 + item.offset,
                    rotation * v2 + item.offset,
                    rotation * v1 + item.offset,
            };
            for (int i = 0; i < 8; i++)
            {
                block.Planes[inBlockId + i] = new GPUItem(vertices[i], item.color);
            }
        }

        protected override void RemoveItem(InfinitePlaneCloudBlock block, int id)
        {
            var inBlockId = (id % InfinitePlaneCloudBlock.Capacity);
            for (int i = 0; i < 8; i++)
            {
                block.Planes[inBlockId + i] = default;
            }
        }
    }
}