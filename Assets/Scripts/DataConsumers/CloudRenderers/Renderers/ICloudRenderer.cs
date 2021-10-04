using System;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface ICloudRenderer<TCloudItem> : IDataConsumer, IDisposable
            where TCloudItem : struct, ICloudItem
    {   
        int ItemsCount { get; }

        void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e);

        void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e);

        void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e);
    }
}