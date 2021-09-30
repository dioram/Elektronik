using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class CubeCloudRenderer : CloudRenderer<SlamMarker, CubeCloudBlock>
    {
        protected override Func<SlamMarker, bool> Filter { get; } = marker => marker.Type == SlamMarker.MarkerType.Cube;

        protected override void ProcessItem(CubeCloudBlock block, SlamMarker item, int inBlockId)
        {
            block.Transforms[inBlockId] = Matrix4x4.TRS(item.Position, item.Rotation, Vector3.one);
            block.Scales[inBlockId] = item.Scale;
            block.Colors[inBlockId] = item.Color;
        }

        protected override void RemoveItem(CubeCloudBlock block, int inBlockId)
        {
            block.Transforms[inBlockId] = default;
            block.Scales[inBlockId] = default;
            block.Colors[inBlockId] = default;
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