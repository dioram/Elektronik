using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Wrapper for GPU lines renderer. Use it to add renderer on Unity scene. </summary>
    internal class SlamLineCloudRendererComponent : CloudRendererComponent<SlamLine>
    {
        [SerializeField] private Shader CloudShader;
        
        public void SetAlpha(float alpha)
        {
            ((SlamLineCloudRenderer)NestedRenderer).SetAlpha(alpha);
        }
        
        private void Awake()
        {
            NestedRenderer = new SlamLineCloudRenderer(CloudShader, Scale);
        }
    }
}