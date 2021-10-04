using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class CubeCloudRendererComponent 
            : CloudRendererComponent<SlamMarker, TransparentMarkerCloudBlock, MarkerCloudBlock.MarkerGpuData>
    {
        private void Awake()
        {
            NestedRenderer = new CubeCloudRenderer(CloudShader);
        }
    }
}