using Elektronik.DataObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Wrapper for GPU markers renderer. Use it to add renderer on Unity scene. </summary>
    internal class MarkerCloudRendererComponent : CloudRendererComponent<SlamMarker>
    {
        private void Awake()
        {
            NestedRenderer = new MarkerCloudRenderer(GetComponentsInChildren<IMarkerCloudRenderer>());
        }
    }
}