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
    public class SlamPackageCommander : RepaintablePackageViewUpdateCommander
    {
        public Map map;

        public override LinkedList<IPackageViewUpdateCommand> GetCommands(IPackage pkg)
        {
            if (pkg.Type != PackageType.SLAMPackage)
                return m_commander?.GetCommands(pkg);
            var commands = new LinkedList<IPackageViewUpdateCommand>();
            var slamPkg = pkg as SlamPackage;

            if (slamPkg.Timestamp == -1)
            {
                commands.AddLast(new ClearCommand(map.PointsContainer, map.LinesContainer, map.ObservationsContainer));
                commands.Last.Value.Execute();
                return commands;
            }

            // При добавлении объектов в карту их в карте быть не должно.
            slamPkg.TestExistent(obj => obj.id != -1 && obj.isNew, map.PointsContainer, map.ObservationsContainer);
            commands.AddLast(new AddCommand(map.PointsContainer, map.LinesContainer, map.ObservationsContainer, slamPkg));
            commands.Last.Value.Execute();

            // При любых манипуляциях с картой объекты, над которыми происходят манипуляции, должны быть в карте.
            slamPkg.TestNonExistent(obj => obj.id != -1, map.PointsContainer, map.ObservationsContainer);
            commands.AddLast(new UpdateCommand(map.PointsContainer, map.ObservationsContainer, slamPkg));
            commands.Last.Value.Execute();

            commands.AddLast(new PostProcessingCommand(map.PointsContainer, map.LinesContainer, map.ObservationsContainer, slamPkg));
            commands.Last.Value.Execute();

            return commands;
        }

        public override void Repaint()
        {
            map.ObservationsContainer.Repaint();
            map.LinesContainer.Repaint();
            map.PointsContainer.Repaint();
        }

        public override void Clear()
        {
            map.PointsContainer.Clear();
            map.LinesContainer.Clear();
            map.ObservationsContainer.Clear();
        }

    }
}
