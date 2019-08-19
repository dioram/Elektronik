using Elektronik.Common.Maps;
using Elektronik.Common.Data;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Tracking;
using System.Collections.Generic;

namespace Elektronik.Offline.Commanders
{
    public class TrackingPackageCommander : PackageViewUpdateCommander
    {
        public TracksOwner tracks;
        public override LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg)
        {
            var commands = new LinkedList<IPackageViewUpdateCommand>();
            if (pkg.Type != PackageType.TrackingPackage)
                return m_commander?.GetCommands(pkg) ?? commands;
            commands.AddLast(new UpdateCommand(tracks.Helmets, pkg as TrackingPackage, tracks.HelmetsPool));
            return commands;
        }

    }
}
