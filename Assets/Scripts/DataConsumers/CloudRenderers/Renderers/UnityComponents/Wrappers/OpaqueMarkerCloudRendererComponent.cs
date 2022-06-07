using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Wrapper for GPU opaque markers renderer. Use it to add renderer on Unity scene. </summary>
    internal class OpaqueMarkerCloudRendererComponent : CloudRendererComponent<SlamMarker>, IMarkerCloudRenderer
    {
        [SerializeField] private Shader CloudShader;

        // ReSharper disable once InconsistentNaming
        [SerializeField] private SlamMarker.MarkerType _markerType;

        /// <inheritdoc />
        public SlamMarker.MarkerType MarkerType => _markerType;

        private void Awake()
        {
            NestedRenderer = new MarkerCloudRenderer<MarkerCloudBlock>(CloudShader, MarkerType, Scale);
        }
    }
}