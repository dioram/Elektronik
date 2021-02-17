using System;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public interface IContainer<T> : IList<T>
    {
        public event Action<IContainer<T>, IEnumerable<T>> ItemsAdded;
        public event Action<IContainer<T>, IEnumerable<T>> ItemsUpdated;
        public event Action<IContainer<T>, IEnumerable<int>> ItemsRemoved;

        T this[T obj] { get; set; }
        void AddRange(IEnumerable<T> objects);
        void Remove(IEnumerable<T> objs);
        bool TryGet(T obj, out T current);
        void UpdateItem(T obj);
        void UpdateItems(IEnumerable<T> objs);
        new void Clear();
    }
}
