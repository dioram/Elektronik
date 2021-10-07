using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class OpaqueMarkerCloudRendererComponent : CloudRendererComponent<SlamMarker>, IMarkerCloudRenderer
    {
        [SerializeField] private Shader CloudShader;

        // ReSharper disable once InconsistentNaming
        [SerializeField] private SlamMarker.MarkerType _markerType;

        public SlamMarker.MarkerType MarkerType => _markerType;

        private void Awake()
        {
            NestedRenderer = new MarkerCloudRenderer<MarkerCloudBlock>(CloudShader, MarkerType, Scale);
        }
    }
}