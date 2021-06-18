using System.Collections.Generic;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Clouds
{
    public interface ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e);

        void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e);

        void OnItemsRemoved(object sender, RemovedEventArgs e);

        void ShowItems(object sender, IEnumerable<TCloudItem> items);

        void OnClear(object sender);
    }
}