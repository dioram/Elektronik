using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class ObservationCloudRenderer : CloudRenderer<SlamObservation, ObservationCloudBlock>
    {
        protected override void ProcessItem(ObservationCloudBlock block, SlamObservation item, int inBlockId)
        {
            float halfSide = ItemSize / 2;
            // TODO: Сделать правильную пирамиду
            var vs = new []
            {
                new Vector3(0, 0, -halfSide),
                new Vector3(0, -halfSide, halfSide),
                new Vector3(halfSide, halfSide, halfSide),
                new Vector3(-halfSide, halfSide, halfSide),
            }.Select(v => item.Rotation * v + item.Point.Position).ToArray();

            var vertices = new[]
            {
                vs[0], vs[1], vs[2],
                vs[0], vs[1], vs[3],
                vs[0], vs[2], vs[3],
                vs[1], vs[2], vs[3],
            };
            for (int i = 0; i < vertices.Length; i++)
            {
                block.Points[inBlockId * vertices.Length + i] = new GPUItem(vertices[i], item.Point.Color);
            }
        }

        protected override void RemoveItem(ObservationCloudBlock block, int inBlockId)
        {
            const int cm = ObservationCloudBlock.CapacityMultiplier;
            for (int i = 0; i < cm; i++)
            {
                block.Points[inBlockId * cm + i] = default;
            }
        }
    }
}