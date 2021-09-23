using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface ICloudRenderer<TCloudItem> : IDataConsumer
            where TCloudItem : struct, ICloudItem
    {
        void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e);

        void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e);

        void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e);
    }
}