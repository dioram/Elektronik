using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Commands.Generic
{
    public class RemoveConnectionsCommand<T> : ICommand where T: ICloudItem
    {
        private readonly IList<(int, int)> _connections;
        private readonly IConnectableObjectsContainer<T> _сontainer;

        public RemoveConnectionsCommand(IConnectableObjectsContainer<T> сontainer, IEnumerable<(int, int)> connections)
        {
            _connections = connections.ToList();
            _сontainer = сontainer;
        }

        public void Execute() => _сontainer.RemoveConnections(_connections);
        public void UnExecute() => _сontainer.AddConnections(_connections);
    }
}
