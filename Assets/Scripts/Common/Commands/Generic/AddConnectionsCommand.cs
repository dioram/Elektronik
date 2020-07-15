using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.Commands.Generic
{
    public class AddConnectionsCommand<T> : ICommand
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
