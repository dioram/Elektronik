using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudRendererComponent : CloudRendererComponent<SlamObservation>
    {
        [SerializeField] private Shader CloudShader;
        
        private void Awake()
        {
            NestedRenderer = new ObservationCloudRenderer(CloudShader);
        }
    }
}