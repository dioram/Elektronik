using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface IMarkerCloudRenderer: ICloudRenderer<SlamMarker>
    {
        public SlamMarker.MarkerType MarkerType { get; }
    }
}