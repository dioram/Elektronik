using System;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base class for rendering object clouds. Such as point cloud, line cloud, etc. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    /// <typeparam name="TCloudBlock"></typeparam>
    /// <typeparam name="TGpuItem"></typeparam>
    public abstract class CloudRendererComponent<TCloudItem, TCloudBlock, TGpuItem>
            : MonoBehaviour, ICloudRenderer<TCloudItem>, IQueueableRenderer
            where TCloudItem : struct, ICloudItem
            where TCloudBlock : class, ICloudBlock<TGpuItem>
    {
        public Shader CloudShader;

        protected CloudRenderer<TCloudItem, TCloudBlock, TGpuItem> NestedRenderer;

        public int ItemsCount => NestedRenderer.ItemsCount;

        #region Unity events

        private void Update()
        {
            NestedRenderer.UpdateDataOnGpu();
        }

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region IQueueableRenderer

        public void RenderNow()
        {
            NestedRenderer.RenderNow();
        }

        public int RenderQueue => NestedRenderer.RenderQueue;

        #endregion

        #region ICloudRenderer

        public float Scale
        {
            get => NestedRenderer.Scale;
            set => NestedRenderer.Scale = value;
        }

        public void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            NestedRenderer.OnItemsAdded(sender, e);
        }

        public void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            NestedRenderer.OnItemsUpdated(sender, e);
        }

        public void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            NestedRenderer.OnItemsRemoved(sender, e);
        }

        #endregion

        public void Dispose()
        {
            NestedRenderer?.Dispose();
        }
    }
}