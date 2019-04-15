using System.Collections;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public interface ICloudObjectsContainer<T> : IEnumerable<T>, IEnumerable
    {
        int Add(T obj);
        void AddRange(T[] objects);
        void ChangeColor(T obj);
        void Clear();
        bool Exists(T obj);
        bool Exists(int objId);
        T this[int id] { get; set; }
        T this[T obj] { get; set; }
        T[] GetAll();
        void Remove(int id);
        void Remove(T obj);
        void Repaint();
        bool TryGet(int idx, out T current);
        bool TryGet(T obj, out T current);
        void Update(T obj);
    }
}
