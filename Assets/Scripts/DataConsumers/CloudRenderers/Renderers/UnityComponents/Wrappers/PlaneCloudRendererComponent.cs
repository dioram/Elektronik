using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Wrapper for GPU planes renderer. Use it to add renderer on Unity scene. </summary>
    internal class PlaneCloudRendererComponent : CloudRendererComponent<SlamPlane>
    {
        [SerializeField] private Shader CloudShader;
        public float ItemSize = 100;
        
        private void Awake()
        {
            NestedRenderer = new PlaneCloudRenderer(CloudShader, Scale, ItemSize);
        }
    }
}