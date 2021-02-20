using Elektronik.Common.Data.PackageObjects;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    /// <summary> Interface of container for connectable objects. </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConnectableObjectsContainer<T> : ICloudObjectsContainer<T> where T: ICloudItem
    {
        IEnumerable<SlamLine> Connections { get; }
        bool AddConnection(int id1, int id2);
        void AddConnections(IEnumerable<(int id1, int id2)> connections);
        bool AddConnection(T obj1, T obj2);
        void AddConnections(IEnumerable<(T obj1, T obj2)> connections);
        bool RemoveConnection(int id1, int id2);
        void RemoveConnections(IEnumerable<(int id1, int id2)> connections);
        bool RemoveConnection(T obj1, T obj2);
        void RemoveConnections(IEnumerable<(T obj1, T obj2)> connections);
        IEnumerable<(int id1, int id2)> GetAllConnections(int id);
        IEnumerable<(int id1, int id2)> GetAllConnections(T obj);
    }
}
