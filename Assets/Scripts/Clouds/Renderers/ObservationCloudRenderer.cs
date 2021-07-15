using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class ObservationCloudRenderer : CloudRenderer<SlamObservation, ObservationCloudBlock>
    {
        private static readonly Vector3[] points = new[]
        {
            new Vector3(0, 0, -0.5f),
            new Vector3(0, Mathf.Sqrt(8 / 9f), 1 / 3f),
            new Vector3(Mathf.Sqrt(2 / 3f), -Mathf.Sqrt(2 / 9f), 1 / 3f),
            new Vector3(-Mathf.Sqrt(2 / 3f), -Mathf.Sqrt(2 / 9f), 1 / 3f),
        };

        protected override void ProcessItem(ObservationCloudBlock block, SlamObservation item, int inBlockId)
        {
            var vs = points.Select(v => item.Rotation * (v * ItemSize) + item.Point.Position).ToArray();
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