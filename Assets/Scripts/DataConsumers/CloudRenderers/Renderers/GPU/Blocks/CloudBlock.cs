using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base implementation of cloud block. </summary>
    /// <typeparam name="TGpuItem"> Type of data that will be sent on GPU. </typeparam>
    internal abstract class CloudBlock<TGpuItem> : ICloudBlock<TGpuItem>
    {
        /// <inheritdoc />
        public abstract int RenderQueue { get; }

        /// <inheritdoc />
        public float Scale { get; set; }

        /// <inheritdoc />
        public virtual void UpdateDataOnGpu()
        {
            Updated = false;
        }

        /// <inheritdoc />
        public virtual void RenderData()
        {
            if (RenderMaterial is null) return;
            RenderMaterial.SetFloat(_scaleShaderProp, Scale);
        }

        public abstract TGpuItem this[int index] { get; set; }

        /// <inheritdoc />
        public abstract void Dispose();

        #region Protected

        protected CloudBlock(float scale)
        {
            Scale = scale;
        }

        /// <summary> Material that renders this block. </summary>
        [CanBeNull] protected Material RenderMaterial;

        /// <summary> Was contend changed? </summary>
        protected bool Updated = false;

        #endregion

        #region Private

        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");

        #endregion
    }
}