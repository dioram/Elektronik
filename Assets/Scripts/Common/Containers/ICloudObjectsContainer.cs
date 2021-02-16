using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Clouds;

namespace Elektronik.Common.Containers
{
    public interface ICloudObjectsContainer<T> : IContainer<T> where T: ICloudItem
    {
        bool Contains(int objId);
        bool TryGet(int idx, out T current);
        bool TryGetAsPoint(int idx, out SlamPoint point);
        bool TryGetAsPoint(T obj, out SlamPoint point);
    }
}
