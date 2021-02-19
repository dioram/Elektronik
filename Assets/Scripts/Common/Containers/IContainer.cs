using System;
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
        public event Action<IContainer<T>, AddedEventArgs<T>> OnAdded;
        public event Action<IContainer<T>, UpdatedEventArgs<T>> OnUpdated;
        public event Action<IContainer<T>, RemovedEventArgs> OnRemoved;

        T this[T obj] { get; set; }
        void AddRange(IEnumerable<T> objects);
        void Remove(IEnumerable<T> objs);
        bool TryGet(T obj, out T current);
        void UpdateItem(T obj);
        void UpdateItems(IEnumerable<T> objs);
    }
}
