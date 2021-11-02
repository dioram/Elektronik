using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationCloudRendererComponent : CloudRendererComponent<SlamObservation>
    {
        [SerializeField] private Shader CloudShader;
        [Range(0.5f, 3)] public float ItemSize = 0.5f;

        public void SetSize(float value)
        {
            ((ObservationCloudRenderer)NestedRenderer).ItemSize = value;
        }

        private void Awake()
        {
            NestedRenderer = new ObservationCloudRenderer(CloudShader, Scale, ItemSize);
        }
    }
}