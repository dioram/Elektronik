using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Wrapper for GPU simple lines renderer. Use it to add renderer on Unity scene. </summary>
    internal class SimpleLineCloudRendererComponent : CloudRendererComponent<SimpleLine>
    {
        [SerializeField] private Shader CloudShader;
        
        public void SetAlpha(float alpha)
        {
            ((SimpleLineCloudRenderer)NestedRenderer).SetAlpha(alpha);
        }
        
        private void Awake()
        {
            NestedRenderer = new SimpleLineCloudRenderer(CloudShader, Scale);
        }
    }
}