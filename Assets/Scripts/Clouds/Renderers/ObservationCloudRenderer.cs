using System.Linq;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class ObservationCloudRenderer : CloudRenderer<SlamObservation, ObservationCloudBlock>
    {
        private static readonly Vector3[] Points = {
            new Vector3(0, 0, -1f),
            new Vector3( Mathf.Sqrt(0.5f),  Mathf.Sqrt(0.5f), 1 / 3f),
            new Vector3( Mathf.Sqrt(0.5f), -Mathf.Sqrt(0.5f), 1 / 3f),
            new Vector3(-Mathf.Sqrt(0.5f), -Mathf.Sqrt(0.5f), 1 / 3f),
            new Vector3(-Mathf.Sqrt(0.5f),  Mathf.Sqrt(0.5f), 1 / 3f),
        };

        protected override void ProcessItem(ObservationCloudBlock block, SlamObservation item, int inBlockId)
        {
            var vs = Points.Select(v => item.Rotation * (v * ItemSize) + item.Point.Position).ToArray();
            var vertices = new[]
            {
                vs[0], vs[1], vs[2],
                vs[0], vs[2], vs[3],
                vs[0], vs[3], vs[4],
                vs[0], vs[4], vs[1],
                vs[1], vs[2], vs[3],
                vs[3], vs[4], vs[1],
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