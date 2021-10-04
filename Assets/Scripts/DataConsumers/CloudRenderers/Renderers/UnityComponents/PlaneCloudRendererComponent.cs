using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PlaneCloudRendererComponent : CloudRendererComponent<SlamPlane, PlaneCloudBlock, GPUItem[]>
    {
        public float ItemSize = 100;
        
        private void Awake()
        {
            NestedRenderer = new PlaneCloudRenderer(CloudShader) { ItemSize = ItemSize };
        }
    }
}