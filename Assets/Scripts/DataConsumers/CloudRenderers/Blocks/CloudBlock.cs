using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public abstract class CloudBlock<TGpuItem> : ICloudBlock<TGpuItem>
    {
        public abstract int RenderQueue { get; }

        public float Scale { get; set; }

        protected CloudBlock(float scale)
        {
            Scale = scale;
        }

        public virtual void UpdateDataOnGpu()
        {
            Updated = false;
        }

        public virtual void RenderData()
        {
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_scaleShaderProp, Scale);
        }

        public abstract TGpuItem this[int index] { get; set; }

        public abstract void Dispose();

        #region Protected

        [CanBeNull] protected Material RenderMaterial;
        protected bool Updated = false;
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");

        #endregion
    }
}