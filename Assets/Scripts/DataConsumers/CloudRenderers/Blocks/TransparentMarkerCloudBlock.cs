using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class TransparentMarkerCloudBlock : MarkerCloudBlock
    {
        public override int RenderQueue => 3000;

        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetPass(1);
            Graphics.DrawProceduralNow(MeshTopology.Points, Capacity);
        }
    }
}