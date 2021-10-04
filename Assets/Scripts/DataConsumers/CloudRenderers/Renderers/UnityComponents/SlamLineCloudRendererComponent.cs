using Elektronik.Data.PackageObjects;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class SlamLineCloudRendererComponent : CloudRendererComponent<SlamLine, LineCloudBlock, (GPUItem begin, GPUItem end)>
    {
        private void Awake()
        {
            NestedRenderer = new SlamLineCloudRenderer(CloudShader);
        }
    }
}