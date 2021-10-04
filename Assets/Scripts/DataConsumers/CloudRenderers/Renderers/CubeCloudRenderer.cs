using System;
using Elektronik.Data.PackageObjects;
using UnityEngine;
using MarkerGpuData = Elektronik.DataConsumers.CloudRenderers.MarkerCloudBlock.MarkerGpuData;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class CubeCloudRenderer : CloudRenderer<SlamMarker, TransparentMarkerCloudBlock, MarkerGpuData>
    {
        public CubeCloudRenderer(Shader shader) : base(shader)
        {
        }
        
        protected override int BlockCapacity => MarkerCloudBlock.Capacity;

        protected override Func<SlamMarker, bool> Filter { get; } = marker => marker.Type == SlamMarker.MarkerType.Cube;

        protected override TransparentMarkerCloudBlock CreateNewBlock() => new TransparentMarkerCloudBlock(Shader);

        protected override void ProcessItem(TransparentMarkerCloudBlock block, SlamMarker item, int inBlockId)
        {
            block[inBlockId] = new MarkerGpuData(Matrix4x4.TRS(item.Position, item.Rotation, Vector3.one),
                                                 item.Scale, item.Color);
        }

        protected override void RemoveItem(TransparentMarkerCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }
    }
}