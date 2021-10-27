using System;
using System.Collections.Generic;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Interface of container for connectable objects. </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConnectableObjectsCloudContainer<T> : ICloudContainer<T> where T : struct, ICloudItem
    {
        /// <summary> List of connections. </summary>
        IEnumerable<SlamLine> Connections { get; }
        
        /// <summary> Add new connections between given objects. </summary>
        /// <param name="connections"> List of pairs of ids of objects that should be connected. </param>
        void AddConnections(IEnumerable<(int id1, int id2)> connections);
        
        /// <summary> Removed connections between given objects. </summary>
        /// <param name="connections"> List of pairs of ids of objects that should be disconnected. </param>
        void RemoveConnections(IEnumerable<(int id1, int id2)> connections);
        
        /// <summary> Get all connections for given object. </summary>
        /// <param name="id"> Id of object. </param>
        /// <returns> List of ids of objects that are connected to given. </returns>
        IEnumerable<int> GetConnections(int id);
        
        /// <summary> Get all connections for given object. </summary>
        /// <param name="obj"> Object. </param>
        /// <returns> List of ids of objects that are connected to given. </returns>
        IEnumerable<int> GetConnections(T obj);

        /// <summary> This event will be raised when any connection was updated. </summary>
        event EventHandler<ConnectionsEventArgs> OnConnectionsUpdated;
        
        /// <summary> This event will be raised when any connection was removed. </summary>
        event EventHandler<ConnectionsEventArgs> OnConnectionsRemoved;
    }
}