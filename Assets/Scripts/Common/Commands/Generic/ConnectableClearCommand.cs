using Elektronik.Common.Containers;
using System.Linq;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Commands.Generic
{
    public class ConnectableClearCommand<T> : ClearCommand<T> where T: ICloudItem
    {
        private readonly (int, int)[] m_connections;
        private readonly IConnectableObjectsContainer<T> m_container;

        public ConnectableClearCommand(IConnectableObjectsContainer<T> container) : base(container)
        {
            m_container = container;
            m_connections = container.Connections.Select(l => (l.pt1.Id, l.pt2.Id)).ToArray();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            m_container.AddConnections(m_connections);
        }
    }
}
