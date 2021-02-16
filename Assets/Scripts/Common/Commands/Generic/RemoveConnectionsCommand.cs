using Elektronik.Common.Containers;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;

namespace Elektronik.Common.Commands.Generic
{
    public class RemoveConnectionsCommand<T> : ICommand where T: ICloudItem
    {
        IList<(int, int)> m_connections;
        IConnectableObjectsContainer<T> m_container;

        public RemoveConnectionsCommand(IConnectableObjectsContainer<T> container, IEnumerable<(int, int)> connections)
        {
            m_connections = connections.ToList();
            m_container = container;
        }

        public void Execute() => m_container.RemoveConnections(m_connections);
        public void UnExecute() => m_container.AddConnections(m_connections);
    }
}
