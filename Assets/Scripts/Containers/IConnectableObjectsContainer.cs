using System;
using System.Collections.Generic;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers
{
    /// <summary> Interface of container for connectable objects. </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConnectableObjectsContainer<T> : IContainer<T> where T : ICloudItem
    {
        IEnumerable<SlamLine> Connections { get; }
        void AddConnections(IEnumerable<(int id1, int id2)> connections);
        void RemoveConnections(IEnumerable<(int id1, int id2)> connections);
        IEnumerable<(int id1, int id2)> GetAllConnections(int id);
        IEnumerable<(int id1, int id2)> GetAllConnections(T obj);

        event EventHandler<ConnectionsEventArgs> OnConnectionsUpdated;
        event EventHandler<ConnectionsEventArgs> OnConnectionsRemoved;
    }

    public static class ConnectableContainerDiffExt
    {
        public static IEnumerable<(int id1, int id2)> GetAllConnections<TCloudItem, TCloudItemDiff>(
            this IConnectableObjectsContainer<TCloudItem> container, TCloudItemDiff diff)
                where TCloudItem : ICloudItem
                where TCloudItemDiff : ICloudItemDiff<TCloudItem>
        {
            return container.GetAllConnections(diff.Apply());
        }
    }
}