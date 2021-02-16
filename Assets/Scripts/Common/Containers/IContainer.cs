using System;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public interface IContainer<T> : IList<T>
    {
        public event Action<IEnumerable<T>> ItemsAdded;
        public event Action<IEnumerable<T>> ItemsUpdated;
        public event Action<IEnumerable<int>> ItemsRemoved;
        public event Action ItemsCleared;

        T this[T obj] { get; set; }
        void AddRange(IEnumerable<T> objects);
        void Remove(IEnumerable<T> objs);
        bool TryGet(T obj, out T current);
        void UpdateItem(T obj);
        void UpdateItems(IEnumerable<T> objs);
    }
}
