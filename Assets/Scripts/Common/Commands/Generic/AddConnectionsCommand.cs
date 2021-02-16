using Elektronik.Common.Containers;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;

namespace Elektronik.Common.Commands.Generic
{
    public class AddConnectionsCommand<T> : ICommand where T: ICloudItem
    {
        protected readonly IList<(int, int)> m_connections;
        protected readonly IConnectableObjectsContainer<T> m_container;

        public AddConnectionsCommand(IConnectableObjectsContainer<T> container, IEnumerable<(int, int)> newConnections)
        {
            m_connections = newConnections.ToList();
            m_container = container;
        }

        public virtual void Execute() => m_container.AddConnections(m_connections);
        public virtual void UnExecute() => m_container.RemoveConnections(m_connections);
    }
}
