using Elektronik.DataObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Interface for renderers of visualisation markers. </summary>
    public interface IMarkerCloudRenderer: ICloudRenderer<SlamMarker>
    {
        /// <summary> Type of supported markers for this renderer. </summary>
        public SlamMarker.MarkerType MarkerType { get; }
    }
}