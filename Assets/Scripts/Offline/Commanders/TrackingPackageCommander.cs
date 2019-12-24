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
        public override void GetCommands(IPackage pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.Type == PackageType.TrackingPackage)
            {
                commands.AddLast(new UpdateCommand(tracks.Helmets, pkg as TrackingPackage, tracks.HelmetsPool));
            }
            base.GetCommands(pkg, in commands);
        }

    }
}
