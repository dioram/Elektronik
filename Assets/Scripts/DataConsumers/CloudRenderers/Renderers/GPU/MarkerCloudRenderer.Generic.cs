using Elektronik.Data.PackageObjects;
using UnityEngine;
using MarkerGpuData = Elektronik.DataConsumers.CloudRenderers.MarkerCloudBlock.MarkerGpuData;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface IMarkerCloudRenderer: ICloudRenderer<SlamMarker>
    {
        public SlamMarker.MarkerType MarkerType { get; }
    }

    public class MarkerCloudRenderer<TCloudBlock> : GpuCloudRenderer<SlamMarker, TCloudBlock, MarkerGpuData>,
                                                    IMarkerCloudRenderer
            where TCloudBlock : MarkerCloudBlock, new()
    {
        public MarkerCloudRenderer(Shader shader, SlamMarker.MarkerType markerType) : base(shader)
        {
            MarkerType = markerType;
        }

        public SlamMarker.MarkerType MarkerType { get; }

        protected override int BlockCapacity => MarkerCloudBlock.Capacity;

        protected override TCloudBlock CreateNewBlock()
        {
            var res = new TCloudBlock();
            res.InitShader(Shader);
            return res;
        }

        protected override void ProcessItem(TCloudBlock block, SlamMarker item, int inBlockId)
        {
            block[inBlockId] = new MarkerGpuData(Matrix4x4.TRS(item.Position, item.Rotation, Vector3.one),
                                                 item.Scale, item.Color);
        }

        protected override void RemoveItem(TCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }
    }
}