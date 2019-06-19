using Elektronik.Common;
using Elektronik.Common.Data;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Tracking;
using System.Collections.Generic;

namespace Elektronik.Offline
{
    public class TrackingPackageCommander : RepaintablePackageViewUpdateCommander
    {
        public Helmet helmetPrefab;
        private IList<Helmet> m_helmets;

        public override void Clear()
        {
            foreach (var helmet in m_helmets)
            {
                MF_AutoPool.Despawn(helmet.gameObject);
            }
            m_helmets.Clear();
        }

        public override LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg)
        {
            var commands = new LinkedList<IPackageViewUpdateCommand>();
            if (pkg.Type != PackageType.TrackingPackage)
                return m_commander?.GetCommands(pkg) ?? commands;
            commands.AddLast(new UpdateCommand(m_helmets, pkg as TrackingPackage, helmetPrefab));
            return commands;
        }

        public override void Repaint()
        {
        }
    }
}
