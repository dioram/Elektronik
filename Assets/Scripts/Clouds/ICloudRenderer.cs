using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Clouds
{
    public interface ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e);

        void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e);

        void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e);
    }
}