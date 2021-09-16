using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class PlaneCloudRenderer : CloudRenderer<SlamPlane, PlaneCloudBlock>
    {
        protected override void ProcessItem(PlaneCloudBlock block, SlamPlane item, int inBlockId)
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

        protected override void RemoveItem(PlaneCloudBlock block, int inBlockId)
        {
            for (int i = 0; i < 8; i++)
            {
                block.Planes[inBlockId * 8 + i] = default;
            }
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