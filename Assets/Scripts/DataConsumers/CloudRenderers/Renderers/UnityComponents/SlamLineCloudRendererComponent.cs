using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SlamLineCloudRendererComponent : CloudRendererComponent<SlamLine>
    {
        [SerializeField] private Shader CloudShader;
        
        private void Awake()
        {
            NestedRenderer = new SlamLineCloudRenderer(CloudShader, Scale);
        }
    }
}