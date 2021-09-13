using System.Collections.Generic;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class AddConnectionsCommand<T> : ICommand where T: struct, ICloudItem
    {
        protected readonly IList<(int, int)> Connections;
        protected readonly IConnectableObjectsContainer<T> Container;

        public AddConnectionsCommand(IConnectableObjectsContainer<T> container, IList<(int, int)> newConnections)
        {
            Connections = newConnections;
            Container = container;
        }

        public virtual void Execute() => Container.AddConnections(Connections);
        public virtual void UnExecute() => Container.RemoveConnections(Connections);
    }
}
