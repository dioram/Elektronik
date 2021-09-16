﻿using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Renderers;

namespace Elektronik.Clouds
{
    public interface ICloudRenderer<TCloudItem> : ISourceRenderer
            where TCloudItem : struct, ICloudItem
    {
        void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e);

        void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e);

        void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e);
    }
}