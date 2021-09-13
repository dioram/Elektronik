﻿using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public abstract class CloudRendererComponent<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        public abstract int ItemsCount { get; }
        public abstract float Scale { get; set; }
        public abstract void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e);
        protected static bool IsSenderVisible(object sender) => (sender as IVisible)?.IsVisible ?? true;
    }
}