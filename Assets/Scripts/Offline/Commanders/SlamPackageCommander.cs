using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Offline.Commanders
{
    public class SlamPackageCommander : PackageViewUpdateCommander
    {
        public SlamMap map;
        public override void GetCommands(IPackage pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.Type != PackageType.SLAMPackage)
            {
                base.GetCommands(pkg, in commands);
                return;
            }

            LinkedListNode<IPackageViewUpdateCommand> lastCmd = null;
            var slamPkg = pkg as SlamPackage;
            if (slamPkg.Timestamp == -1)
            {
                lastCmd = commands.AddLast(new ClearCommand(map.PointsContainer, map.LinesContainer, map.ObservationsContainer));
                lastCmd.Value.Execute();
                base.GetCommands(pkg, in commands);
                return;
            }

            // When adding objects to a map they must not be in a map.
            // При добавлении объектов в карту их в карте быть не должно.
            slamPkg.TestExistent(obj => obj.id != -1 && obj.isNew, map.PointsContainer, map.ObservationsContainer);
            lastCmd = commands.AddLast(new AddCommand(map.PointsContainer, map.LinesContainer, map.ObservationsContainer, slamPkg));
            lastCmd.Value.Execute();

            // While map manipulating, manipulated objects must be in a map.
            // При любых манипуляциях с картой объекты, над которыми происходят манипуляции, должны быть в карте.
            slamPkg.TestNonExistent(obj => obj.id != -1, map.PointsContainer, map.ObservationsContainer);
            lastCmd = commands.AddLast(new UpdateCommand(map.PointsContainer, map.ObservationsContainer, slamPkg));
            lastCmd.Value.Execute();

            lastCmd = commands.AddLast(new PostProcessingCommand(map.PointsContainer, map.LinesContainer, map.ObservationsContainer, slamPkg));
            lastCmd.Value.Execute();

            base.GetCommands(pkg, in commands);
        }


    }
}
