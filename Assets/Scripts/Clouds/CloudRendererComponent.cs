using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public abstract class CloudRendererComponent<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem> 
            where TCloudItem : struct, ICloudItem
    {
        public abstract void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e);
        public abstract void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e);
        public abstract void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e);
    }
}