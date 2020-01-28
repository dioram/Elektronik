using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Slam
{
    public class UpdateConnectionsCommand : IPackageViewUpdateCommand
    {
        private readonly SlamLine2[] m_connections2Restore;
        private readonly IConnectionsContainer<SlamLine2> m_connections;
        private readonly SlamPoint[] m_pts;
        private readonly bool m_isRemoved;

        private class ConnectionsComparer : IEqualityComparer<SlamLine2>
        {
            public bool Equals(SlamLine2 x, SlamLine2 y) =>
                x.pt1.id == y.pt1.id && x.pt2.id == y.pt2.id ||
                x.pt1.id == y.pt2.id && x.pt2.id == y.pt1.id;
            public int GetHashCode(SlamLine2 obj) => obj.GetHashCode();
        }

        public UpdateConnectionsCommand(IConnectionsContainer<SlamLine2> connections, ISlamActionPackage changedObjs)
        {
            m_isRemoved = changedObjs.ActionType == Data.ActionType.Remove;
            switch (changedObjs.ObjectType)
            {
                case Data.ObjectType.Observation:
                    m_pts = ((ActionDataPackage<SlamObservation>)changedObjs).Objects.Select(o => o.Point).ToArray();
                    break;
                case Data.ObjectType.Point:
                    m_pts = ((ActionDataPackage<SlamPoint>)changedObjs).Objects;
                    break;
                default:
                    throw new ArgumentException("Unsupported object type");
            }
            m_connections = connections;
            IEnumerable<SlamLine2> connections2Restore = Enumerable.Empty<SlamLine2>();
            foreach (var pckgPt in m_pts)
            {
                connections2Restore = connections2Restore.Concat(m_connections[pckgPt]);
            }
            m_connections2Restore = connections2Restore.Distinct(new ConnectionsComparer()).ToArray();
        }

        public void Execute()
        {
            foreach(var pt in m_pts)
            {
                if (m_isRemoved)
                {
                    m_connections.Remove(pt.id);
                }
                else
                {
                    m_connections.Update(pt);
                }
            }
        }

        public void UnExecute()
        {
            if (m_isRemoved)
            {
                m_connections.Add(m_connections2Restore);
            }
            else
            {
                foreach (var connection in m_connections2Restore)
                {
                    m_connections.Update(connection);
                }
            }
        }
    }
}
