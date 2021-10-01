using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class TransparentMarkerCloudBlock : MarkerCloudBlock
    {
        public override int RenderQueue => 3000;

        public TransparentMarkerCloudBlock(Shader shader) : base(shader)
        {
        }

        public override void RenderData()
        {
            base.RenderData();
            RenderMaterial.SetPass(1);
            Graphics.DrawProceduralNow(MeshTopology.Points, Capacity);
        }
    }
}