using System;
using System.Collections.Generic;
using Elektronik.Containers.EventArgs;
using JetBrains.Annotations;

namespace Elektronik.Containers
{
    /// <summary>
    /// Interface of container that allows batched adding, updating, and removing of its elements.
    /// Also rises events on adding, updating, and removing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T> : IList<T>
    {
        [CanBeNull] event EventHandler<AddedEventArgs<T>> OnAdded;

        [CanBeNull] event EventHandler<UpdatedEventArgs<T>> OnUpdated;

        [CanBeNull] event EventHandler<RemovedEventArgs> OnRemoved;

        void AddRange(IEnumerable<T> items);
        void Remove(IEnumerable<T> items);
        void Update(T item);
        void Update(IEnumerable<T> items);
    }
}