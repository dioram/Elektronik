using System;
using System.Collections.Generic;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Interface of container for connectable objects. </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConnectableObjectsContainer<T> : IContainer<T> where T : struct, ICloudItem
    {
        IEnumerable<SlamLine> Connections { get; }
        void AddConnections(IEnumerable<(int id1, int id2)> connections);
        void RemoveConnections(IEnumerable<(int id1, int id2)> connections);
        IEnumerable<(int id1, int id2)> GetAllConnections(int id);
        IEnumerable<(int id1, int id2)> GetAllConnections(T obj);

        event EventHandler<ConnectionsEventArgs> OnConnectionsUpdated;
        event EventHandler<ConnectionsEventArgs> OnConnectionsRemoved;
    }
}