using Elektronik.Common;
using Elektronik.Common.Data;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Tracking;
using System.Collections.Generic;

namespace Elektronik.Offline
{
    public class TrackingPackageCommander : PackageViewUpdateCommander
    {
        public override LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg)
        {
            var commands = new LinkedList<IPackageViewUpdateCommand>();
            if (pkg.Type != PackageType.TrackingPackage)
                return m_commander?.GetCommands(pkg) ?? commands;
            var trackingMap = map as TrackingMap;
            commands.AddLast(new UpdateCommand(trackingMap.m_helmets, pkg as TrackingPackage, trackingMap.m_helmetsPool));
            return commands;
        }

    }
}
