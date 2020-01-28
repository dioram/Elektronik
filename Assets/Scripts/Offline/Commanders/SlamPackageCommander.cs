using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;
using System.Linq;
using System;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Containers;

namespace Elektronik.Offline.Commanders
{
    public class SlamPackageCommander : PackageViewUpdateCommander
    {
        public SlamMap map;

        private void GetCommands<T>(
            ISlamActionPackage pkg, 
            IContainer<T> container, 
            IConnectionsContainer<SlamLine2> connections, 
            in LinkedList<IPackageViewUpdateCommand> commands)
        {
            switch(pkg.ActionType)
            {
                case ActionType.Create:
                    commands.AddLast(new AddCommand<T>(container, (ActionDataPackage<T>)pkg)).Value.Execute();
                    break;
                case ActionType.Move:
                case ActionType.Tint:
                case ActionType.Fuse:
                    commands.AddLast(new UpdateCommand<T>(container, (ActionDataPackage<T>)pkg)).Value.Execute();
                    if (connections != null)
                        commands.AddLast(new UpdateConnectionsCommand(connections, pkg)).Value.Execute();
                    break;
                //case ActionType.Connect:
                case ActionType.Remove:
                    commands.AddLast(new RemoveCommand<T>(container, (ActionDataPackage<T>)pkg)).Value.Execute();
                    if (connections != null)
                        commands.AddLast(new UpdateConnectionsCommand(connections, pkg)).Value.Execute();
                    break;
                case ActionType.Clear:
                    commands.AddLast(new ClearCommand<T>(container));
                    break;
            }
        }

        public override void GetCommands(IPackage pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.PackageType != PackageType.SLAMPackage)
            {
                base.GetCommands(pkg, in commands);
                return;
            }

            ISlamActionPackage slamPckg = pkg as ISlamActionPackage;

            switch(slamPckg.ObjectType)
            {
                case ObjectType.Point:
                    GetCommands(slamPckg, map.PointsContainer, map.PointsConnections, in commands);
                    break;
                case ObjectType.Observation:
                    GetCommands(slamPckg, map.ObservationsContainer, map.ObservationsConnections, in commands);
                    break;
                case ObjectType.Line:
                    GetCommands(slamPckg, map.LinesContainer, null, in commands);
                    break;
            }

            base.GetCommands(pkg, in commands);
        }


    }
}
