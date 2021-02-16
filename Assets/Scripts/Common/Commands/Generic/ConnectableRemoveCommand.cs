using Elektronik.Common.Containers;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;

namespace Elektronik.Common.Commands.Generic
{
    public class ConnectableRemoveCommand<T> : RemoveCommand<T> where T: ICloudItem
    {
        private IList<(int, int)> m_connections;
        private IConnectableObjectsContainer<T> m_container;

        public ConnectableRemoveCommand(IConnectableObjectsContainer<T> container, IEnumerable<T> objects) : base(container, objects)
        {
            m_container = container;
            m_connections = objects.SelectMany(o => m_container.GetAllConnections(o)).ToList();
        }

        public override void UnExecute()
        {
            base.UnExecute();
            if (m_connections != null)
            {
                m_container.AddConnections(m_connections);
            }
        }
    }
}
