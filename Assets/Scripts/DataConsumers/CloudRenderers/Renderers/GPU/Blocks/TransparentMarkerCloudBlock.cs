using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    
    /// <summary> Implementation of cloud block for rendering transparent markers. </summary>
    internal class TransparentMarkerCloudBlock : MarkerCloudBlock
    {
        /// <inheritdoc />
        public override int RenderQueue => 3000;

        /// <inheritdoc />
        public override void RenderData()
        {
            base.RenderData();
            if (RenderMaterial is null) return;
            RenderMaterial.SetPass(1);
            Graphics.DrawProceduralNow(MeshTopology.Points, Capacity);
        }
    }
}