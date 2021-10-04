using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MarkerCloudRendererComponent : CloudRendererComponent<SlamMarker>
    {
        private void Awake()
        {
            NestedRenderer = new MarkerCloudRenderer(GetComponentsInChildren<IMarkerCloudRenderer>());
        }
    }
}