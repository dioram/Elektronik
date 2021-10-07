using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PlaneCloudRendererComponent : CloudRendererComponent<SlamPlane>
    {
        [SerializeField] private Shader CloudShader;
        public float ItemSize = 100;
        
        private void Awake()
        {
            NestedRenderer = new PlaneCloudRenderer(CloudShader, Scale, ItemSize);
        }
    }
}