using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Containers
{
    public class ConnectionsManager<T>
    {
        private readonly IContainer<SlamLine2> m_connections;
        private readonly ICloudObjectsContainer<T> m_objects;
        public ConnectionsManager(IContainer<SlamLine2> connections, ICloudObjectsContainer<T> objects)
        {
            m_connections = connections;
            m_objects = objects;
        }
        public void Update()
        {
            foreach (var connection in m_connections)
            {
                if (m_objects.TryGetAsPoint(connection.pt1.id, out var pt1) &&
                    m_objects.TryGetAsPoint(connection.pt2.id, out var pt2))
                {
                    var updatedConn = new SlamLine2(pt1, pt2);
                    m_connections.Update(updatedConn);
                }
                else
                {
                    m_connections.Remove(connection);
                }
            }
        }
    }
}
