using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base wrapper for any GPU renderers. Use it to add renderer on Unity scene. </summary>
    /// <typeparam name="TCloudItem"> Type of items it can render. </typeparam>
    internal abstract class CloudRendererComponent<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem>, IGpuRenderer
            where TCloudItem : struct, ICloudItem
    {
        #region Unity events

        private void Update()
        {
            UpdateDataOnGpu();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region IGpuRenderer

        /// <inheritdoc />
        public void UpdateDataOnGpu()
        {
            (NestedRenderer as IGpuRenderer)?.UpdateDataOnGpu();
        }

        /// <inheritdoc />
        public void RenderNow()
        {
            (NestedRenderer as IGpuRenderer)?.RenderNow();
        }

        /// <inheritdoc />
        public int RenderQueue => (NestedRenderer as IGpuRenderer)?.RenderQueue ?? 0;

        #endregion

        #region ICloudRenderer

        /// <inheritdoc />
        public int ItemsCount => NestedRenderer.ItemsCount;

        /// <inheritdoc />
        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                NestedRenderer.Scale = value;
            }
        }

        /// <inheritdoc />
        public void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            NestedRenderer.OnItemsAdded(sender, e);
        }

        /// <inheritdoc />
        public void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            NestedRenderer.OnItemsUpdated(sender, e);
        }

        /// <inheritdoc />
        public void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            NestedRenderer.OnItemsRemoved(sender, e);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            NestedRenderer?.Dispose();
        }

        #endregion

        #region Protected

        /// <summary> Renderer that actually do all work. </summary>
        protected ICloudRenderer<TCloudItem> NestedRenderer;

        #endregion

        #region Private

        private float _scale = 1;

        #endregion
    }
}