using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudRenderer : CloudRenderer<SlamObservation, ObservationCloudBlock>
    {
        protected override void ProcessItem(ObservationCloudBlock block, SlamObservation item, int inBlockId)
        {
            block.Transforms[inBlockId] = Matrix4x4.TRS(item.Point.Position, item.Rotation, Vector3.one);
            block.Colors[inBlockId] = item.Point.Color;
        }

        protected override void RemoveItem(ObservationCloudBlock block, int inBlockId)
        {
            block.Transforms[inBlockId] = default;
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