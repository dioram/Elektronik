using System;
using System.Collections.Generic;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using JetBrains.Annotations;

namespace Elektronik.DataSources.Containers
{
    /// <summary>
    /// Interface of container that allows batched adding, updating, and removing of its elements.
    /// Also rises events on adding, updating, and removing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T> : IList<T> where T : struct, ICloudItem
    {
        [CanBeNull] event EventHandler<AddedEventArgs<T>> OnAdded;

        [CanBeNull] event EventHandler<UpdatedEventArgs<T>> OnUpdated;

        [CanBeNull] event EventHandler<RemovedEventArgs<T>> OnRemoved;

        void AddRange(IList<T> items);
        void Remove(IList<T> items);
        
        /// <summary> Removes items by their ids. </summary>
        /// <param name="itemIds"></param>
        /// <returns> List of removed items. </returns>
        IList<T> Remove(IList<int> itemIds);
        void Update(T item);
        void Update(IList<T> items);
        bool Contains(int id);
    }
}