using System;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    /// <summary> Base class for data recorder plugins. </summary>
    public abstract class DataRecorderPluginBase : IDataRecorderPlugin, ICloudRenderer<SlamPoint>,
                                                   ICloudRenderer<SlamLine>, ICloudRenderer<SimpleLine>,
                                                   ICloudRenderer<SlamObservation>, ICloudRenderer<SlamTrackedObject>,
                                                   ICloudRenderer<SlamPlane>
    {
        #region ICloudRenderer

        /// <inheritdoc cref="ICloudRenderer{TCloudItem}.ItemsCount" />
        public int ItemsCount => 0;

        /// <inheritdoc />
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamPoint> e);

        /// <inheritdoc />
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPoint> e);

        /// <inheritdoc />
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamPoint> e);

        /// <inheritdoc />
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamLine> e);

        /// <inheritdoc />
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamLine> e);

        /// <inheritdoc />
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamLine> e);

        /// <inheritdoc />
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SimpleLine> e);

        /// <inheritdoc />
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SimpleLine> e);

        /// <inheritdoc />
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SimpleLine> e);

        /// <inheritdoc />
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamObservation> e);

        /// <inheritdoc />
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamObservation> e);

        /// <inheritdoc />
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamObservation> e);

        /// <inheritdoc />
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamTrackedObject> e);

        /// <inheritdoc />
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamTrackedObject> e);

        /// <inheritdoc />
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamTrackedObject> e);

        /// <inheritdoc />
        public abstract void OnItemsAdded(object sender, AddedEventArgs<SlamPlane> e);

        /// <inheritdoc />
        public abstract void OnItemsUpdated(object sender, UpdatedEventArgs<SlamPlane> e);

        /// <inheritdoc />
        public abstract void OnItemsRemoved(object sender, RemovedEventArgs<SlamPlane> e);

        #endregion

        #region IDataRecorderPlugin

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public abstract SettingsBag Settings { get; }

        /// <inheritdoc />
        public abstract Texture2D Logo { get; }

        /// <inheritdoc />
        public void Update(float delta)
        {
            // Do nothing
        }

        /// <inheritdoc />
        public float Scale { get; set; }

        /// <inheritdoc />
        public event Action OnDisposed;

        /// <inheritdoc />
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