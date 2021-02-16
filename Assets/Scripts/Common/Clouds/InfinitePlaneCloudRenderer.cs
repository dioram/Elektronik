using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class InfinitePlaneCloudRenderer : CloudRenderer<SlamInfinitePlane, InfinitePlaneCloudBlock>
    {
        protected override void ProcessItem(InfinitePlaneCloudBlock block, SlamInfinitePlane item, int inBlockId)
        {
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
                block.Planes[inBlockId * 8 + i] = new GPUItem(vertices[i], item.color);
            }
        }

        protected override void RemoveItem(InfinitePlaneCloudBlock block, int inBlockId)
        {
            for (int i = 0; i < 8; i++)
            {
                block.Planes[inBlockId * 8 + i] = default;
            }
        }
    }
}