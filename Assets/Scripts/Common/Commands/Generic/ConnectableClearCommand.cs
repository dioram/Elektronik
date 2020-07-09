using Elektronik.Common.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Commands.Generic
{
    public class ConnectableClearCommand<T> : ClearCommand<T>
    {
        private readonly (int, int)[] m_connections;
        private readonly IConnectableObjectsContainer<T> m_container;

        public ConnectableClearCommand(IConnectableObjectsContainer<T> container) : base(container)
        {
            m_container = container;
            m_connections = container.Connections.Select(l => (l.pt1.id, l.pt2.id)).ToArray();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            m_container.AddConnections(m_connections);
        }
    }
}
