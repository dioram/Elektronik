using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public interface IContainer<T> : IEnumerable<T>, IEnumerable
    {
        int Count { get; }
        T this[T obj] { get; set; }
        void Add(T obj);
        void Add(IEnumerable<T> objects);
        void Clear();
        bool Exists(T obj);
        T[] GetAll();
        void Remove(T obj);
        void Remove(IEnumerable<T> objs);
        bool TryGet(T obj, out T current);
        void Update(T obj);
        void Update(IEnumerable<T> objs);
    }
}
