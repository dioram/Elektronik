using System;
using System.Collections.Generic;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using JetBrains.Annotations;

namespace Elektronik.DataSources.Containers
{
    /// <summary>
    /// Interface of container that allows batched adding, updating, and removing of its elements.
    /// Also rises events on adding, updating, and removing.
    /// </summary>
    /// <typeparam name="T"> Type of cloud objects. </typeparam>
    public interface ICloudContainer<T> : IList<T>, IDataSource
            where T : struct, ICloudItem
    {
        /// <summary> Event that will be raised on every adding to this container. </summary>
        [CanBeNull]
        event EventHandler<AddedEventArgs<T>> OnAdded;

        /// <summary> Event that will be raised on every updating of this container. </summary>
        [CanBeNull]
        event EventHandler<UpdatedEventArgs<T>> OnUpdated;

        /// <summary> Event that will be raised on every removing this container. </summary>
        [CanBeNull]
        event EventHandler<RemovedEventArgs<T>> OnRemoved;

        /// <summary> Batched adding of new items. </summary>
        /// <param name="items"> List of new items. </param>
        void AddRange(IList<T> items);

        /// <summary> Batched removing of items. </summary>
        /// <param name="items"> List of removing items. </param>
        void Remove(IList<T> items);

        /// <summary> Batched removing of items by their ids. </summary>
        /// <param name="itemIds"> List of id of removing items .</param>
        /// <returns> List of removed items. </returns>
        IList<T> Remove(IList<int> itemIds);

        /// <summary> Update item in container. </summary>
        /// <param name="item"> Updating item. </param>
        void Update(T item);
        
        /// <summary> Batched updating of items. </summary>
        /// <param name="items"> List of updating items. </param>
        void Update(IList<T> items);
        
        /// <summary> Checks if this container has an item with given id. </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Contains(int id);

        // This declaration is necessary because both IList<T>, IDataSource has method Clear()
        // and this is causing possible ambiguity.
        /// <summary> Clears content of this container. </summary>
        new void Clear();
    }
}