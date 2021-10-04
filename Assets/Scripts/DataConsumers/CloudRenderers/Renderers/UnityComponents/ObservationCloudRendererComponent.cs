using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudRendererComponent
            : CloudRendererComponent<SlamObservation, ObservationCloudBlock, (Matrix4x4 transform, Color color)>
    {
        private void Awake()
        {
            NestedRenderer = new ObservationCloudRenderer(CloudShader);
        }
    }
}