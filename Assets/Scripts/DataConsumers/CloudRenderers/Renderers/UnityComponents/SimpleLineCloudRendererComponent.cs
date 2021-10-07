using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SimpleLineCloudRendererComponent : CloudRendererComponent<SimpleLine>
    {
        [SerializeField] private Shader CloudShader;
        
        private void Awake()
        {
            NestedRenderer = new SimpleLineCloudRenderer(CloudShader, Scale);
        }
    }
}