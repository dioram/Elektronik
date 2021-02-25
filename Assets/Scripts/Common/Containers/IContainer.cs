﻿using System;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    /// <summary>
    /// Interface of container that allows batched adding, updating, and removing of its elements.
    /// Also rises events on adding, updating, and removing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T> : IList<T>
    {
        event Action<IContainer<T>, AddedEventArgs<T>> OnAdded;
        event Action<IContainer<T>, UpdatedEventArgs<T>> OnUpdated;
        event Action<IContainer<T>, RemovedEventArgs> OnRemoved;

        void AddRange(IEnumerable<T> items);
        void Remove(IEnumerable<T> items);
        void UpdateItem(T item);
        void UpdateItems(IEnumerable<T> items);
    }
}
