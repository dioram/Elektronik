using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public interface IConnectableObjectsContainer<T> : ICloudObjectsContainer<T>
    {
        IEnumerable<SlamLine> Connections { get; }
        bool AddConnection(int id1, int id2);
        bool AddConnections(IEnumerable<(int id1, int id2)> connections);
        bool AddConnection(T obj1, T obj2);
        bool AddConnections(IEnumerable<(T obj1, T obj2)> connections);
        bool RemoveConnection(int id1, int id2);
        bool RemoveConnections(IEnumerable<(int id1, int id2)> connections);
        bool RemoveConnection(T obj1, T obj2);
        bool RemoveConnections(IEnumerable<(T obj1, T obj2)> connections);
        IEnumerable<(int id1, int id2)> GetAllConnections(int id);
        IEnumerable<(int id1, int id2)> GetAllConnections(T obj);
    }
}
