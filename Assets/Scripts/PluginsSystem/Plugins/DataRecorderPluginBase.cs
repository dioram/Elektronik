using System;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    public abstract class DataRecorderPluginBase : IDataRecorderPlugin, ICloudRenderer<SlamPoint>,
                                                   ICloudRenderer<SlamLine>, ICloudRenderer<SimpleLine>,
                                                   ICloudRenderer<SlamObservation>, ICloudRenderer<SlamTrackedObject>,
                                                   ICloudRenderer<SlamPlane>
    {
        #region ICloudRenderer

        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamPoint> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPoint> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamPoint> e);
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamLine> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamLine> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamLine> e);
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SimpleLine> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SimpleLine> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SimpleLine> e);
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamObservation> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamObservation> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamObservation> e);
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamTrackedObject> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamTrackedObject> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamTrackedObject> e);
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamPlane> e);
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPlane> e);
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamPlane> e);

        #endregion

        #region IDataRecorderPlugin

        public abstract string DisplayName { get; }

        public abstract SettingsBag Settings { get; }

        public abstract Texture2D Logo { get; }

        public void Update(float delta)
        {
            // Do nothing
        }

        public float Scale { get; set; }

        public event Action OnDisposed;

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            OnDisposed?.Invoke();
            IsDisposed = true;
        }

        #endregion

        #region Protected

        protected bool IsDisposed;
        
        #endregion
    }
}