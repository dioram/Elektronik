using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public abstract class CloudBlock<TGpuItem>: ICloudBlock<TGpuItem>
    {
        public abstract int RenderQueue { get; }

        public float Scale { get; set; } = 1;

        public virtual void UpdateDataOnGPU()
        {
            Updated = false;
        }

        public virtual void RenderData()
        {
            RenderMaterial.SetFloat(_scaleShaderProp, Scale);
        }
        
        public abstract TGpuItem this[int index] { get; set; }

        public abstract void Dispose();

        #region Protected

        protected Material RenderMaterial;
        protected bool Updated = false;
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");

        #endregion
    }
}