using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public interface ICloudObjectsContainer<T> : IContainer<T> where T: ICloudItem
    {
        /// <summary> Check existing of node by id. </summary>
        /// <param name="objId"> Id of node. </param>
        /// <returns> true if exists, otherwise false </returns>
        bool Contains(int objId);
        
        bool TryGet(int idx, out T current);
        
        bool TryGetAsPoint(int idx, out SlamPoint point);
        
        bool TryGetAsPoint(T obj, out SlamPoint point);
    }
}
