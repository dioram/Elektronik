using Elektronik.Common.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Commands.Generic
{
    public class RemoveConnectionsCommand<T> : ICommand
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
