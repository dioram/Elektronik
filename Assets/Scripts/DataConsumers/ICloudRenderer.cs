using System;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataConsumers
{
    /// <summary> Interface for data consumers who renders cloud data on scene. </summary>
    /// <typeparam name="TCloudItem"> Type of supported cloud data. </typeparam>
    public interface ICloudRenderer<TCloudItem> : IDataConsumer, IScalable, IDisposable
            where TCloudItem : struct, ICloudItem
    {
        /// <summary> Amount of rendered objects. </summary>
        int ItemsCount { get; }

        // TODO: sender can be null

        /// <summary> Adds new objects for rendering. </summary>
        /// <remarks>
        /// Two objects with same id but from different sender will be handled as different objects.
        /// </remarks>
        /// <param name="sender"> Sender of objects. </param>
        /// <param name="e"> Event argument with list of objects to add. </param>
        void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e);

        /// <summary> Updates rendering objects. </summary>
        /// <remarks>
        /// Two objects with same id but from different sender will be handled as different objects.
        /// </remarks>
        /// <param name="sender"> Sender of objects. </param>
        /// <param name="e"> Event argument with list of objects to update. </param>
        void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e);

        /// <summary> Removed rendering objects. </summary>
        /// <remarks>
        /// Two objects with same id but from different sender will be handled as different objects.
        /// </remarks>
        /// <param name="sender"> Sender of objects. </param>
        /// <param name="e"> Event argument with list of objects to update. </param>
        void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e);
    }
}