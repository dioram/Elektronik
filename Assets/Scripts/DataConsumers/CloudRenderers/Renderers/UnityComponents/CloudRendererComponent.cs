﻿using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base class for rendering object clouds. Such as point cloud, line cloud, etc. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    public abstract class CloudRendererComponent<TCloudItem>
            : MonoBehaviour, ICloudRenderer<TCloudItem>, IGpuRenderer
            where TCloudItem : struct, ICloudItem
    {
        protected ICloudRenderer<TCloudItem> NestedRenderer;

        public int ItemsCount => NestedRenderer.ItemsCount;

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

        public void UpdateDataOnGpu()
        {
            (NestedRenderer as IGpuRenderer)?.UpdateDataOnGpu();
        }

        public void RenderNow()
        {
            (NestedRenderer as IGpuRenderer)?.RenderNow();
        }

        public int RenderQueue => (NestedRenderer as IGpuRenderer)?.RenderQueue ?? 0;

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