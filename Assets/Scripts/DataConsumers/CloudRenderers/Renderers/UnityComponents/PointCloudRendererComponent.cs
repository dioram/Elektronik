using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class PointCloudRendererComponent : CloudRendererComponent<SlamPoint>
    {
        [SerializeField] private Shader CloudShader;
        [Range(0, 1)] public float ItemSize = 0.05f;
        
        public void SetSize(float value)
        {
            ((PointCloudRenderer)NestedRenderer).ItemSize = value;
        }
        
        private void Awake()
        {
            NestedRenderer = new PointCloudRenderer(CloudShader, Scale, ItemSize);
        }
    }
}