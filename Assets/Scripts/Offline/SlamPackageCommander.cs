using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamPackageCommander : PackageViewUpdateCommander
    {
        public override LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg)
        {
            if (pkg.Type != PackageType.SLAMPackage)
                return m_commander?.GetCommands(pkg);
            var commands = new LinkedList<IPackageViewUpdateCommand>();
            var slamPkg = pkg as SlamPackage;
            var slamMap = map as SlamMap;
            if (slamPkg.Timestamp == -1)
            {
                commands.AddLast(new ClearCommand(slamMap.PointsContainer, slamMap.LinesContainer, slamMap.ObservationsContainer));
                commands.Last.Value.Execute();
                return commands;
            }

            // При добавлении объектов в карту их в карте быть не должно.
            slamPkg.TestExistent(obj => obj.id != -1 && obj.isNew, slamMap.PointsContainer, slamMap.ObservationsContainer);
            commands.AddLast(new AddCommand(slamMap.PointsContainer, slamMap.LinesContainer, slamMap.ObservationsContainer, slamPkg));
            commands.Last.Value.Execute();

            // При любых манипуляциях с картой объекты, над которыми происходят манипуляции, должны быть в карте.
            slamPkg.TestNonExistent(obj => obj.id != -1, slamMap.PointsContainer, slamMap.ObservationsContainer);
            commands.AddLast(new UpdateCommand(slamMap.PointsContainer, slamMap.ObservationsContainer, slamPkg));
            commands.Last.Value.Execute();

            commands.AddLast(new PostProcessingCommand(slamMap.PointsContainer, slamMap.LinesContainer, slamMap.ObservationsContainer, slamPkg));
            commands.Last.Value.Execute();

            return commands;
        }


    }
}
