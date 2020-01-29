using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class RemoveFromConnectionsCommand : IPackageViewUpdateCommand
    {
        private readonly int[] m_ptIds;
        private readonly SlamLine[] m_connections2Restore;
        private readonly IConnectionsContainer<SlamLine> m_connections;

        private class ConnectionsComparer : IEqualityComparer<SlamLine>
        {
            public bool Equals(SlamLine x, SlamLine y) =>
                x.pt1.id == y.pt1.id && x.pt2.id == y.pt2.id ||
                x.pt1.id == y.pt2.id && x.pt2.id == y.pt1.id;
            public int GetHashCode(SlamLine obj) => obj.GetHashCode();
        }

        public RemoveFromConnectionsCommand(IConnectionsContainer<SlamLine> connections, IEnumerable<SlamPoint> removedVertices)
        {
            m_connections = connections;
            m_ptIds = removedVertices.Select(pt => pt.id).ToArray();

            IEnumerable<SlamLine> connections2Restore = Enumerable.Empty<SlamLine>();
            foreach (var pckgPt in m_ptIds)
            {
                connections2Restore = connections2Restore.Concat(m_connections[pckgPt]);
            }
            m_connections2Restore = connections2Restore.Distinct(new ConnectionsComparer()).ToArray();
        }

        public void Execute()
        {
            if (m_ptIds != null)
            {
                m_connections.Remove(m_ptIds);
            }
        }

        public void UnExecute()
        {
            if (m_connections2Restore != null)
            {
                m_connections.Add(m_connections2Restore);
            }
        }
    }
}
