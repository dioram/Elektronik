﻿using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
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

            var rotation = Quaternion.FromToRotation(Vector3.up, item.Normal);

            var vertices = new[]
            {
                rotation * v1 + item.Offset,
                rotation * v2 + item.Offset,
                rotation * v3 + item.Offset,
                rotation * v4 + item.Offset,
                rotation * v4 + item.Offset,
                rotation * v3 + item.Offset,
                rotation * v2 + item.Offset,
                rotation * v1 + item.Offset,
            };
            for (int i = 0; i < 8; i++)
            {
                block.Planes[inBlockId * 8 + i] = new GPUItem(vertices[i], item.Color);
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