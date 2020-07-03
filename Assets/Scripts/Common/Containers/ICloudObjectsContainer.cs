using Elektronik.Common.Data.PackageObjects;
using System.Collections;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public interface ICloudObjectsContainer<T> : IContainer<T>
    {
        bool Exists(int objId);
        T this[int id] { get; set; }
        void Remove(int id);
        bool TryGet(int idx, out T current);
        bool TryGetAsPoint(int idx, out SlamPoint point);
        bool TryGetAsPoint(T obj, out SlamPoint point);
    }
}
