using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SimpleLineCloudRendererComponent 
            : CloudRendererComponent<SimpleLine, LineCloudBlock, (GPUItem begin, GPUItem end)>
    {
        private void Awake()
        {
            NestedRenderer = new SimpleLineCloudRenderer(CloudShader);
        }
    }
}