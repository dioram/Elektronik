using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Offline.Commanders.Slam
{
    public abstract class ConnectableObjectsCommander<T> : ObjectsCommander<T>
    {
        public abstract IConnectionsContainer<SlamLine> ConnectionsMap { get; }
        public abstract SlamPoint Obj2Pt(T obj);
        public virtual SlamLine ClearLines(SlamLine line)
        {
            SlamPoint pt1 = line.pt1; pt1.color = Color.red;
            SlamPoint pt2 = line.pt2; pt2.color = Color.red;
            return new SlamLine(pt1, pt2);
        }
        public override IPackageViewUpdateCommand GetCommand(ISlamActionPackage objectsPackage)
        {
            IPackageViewUpdateCommand objectCommand = base.GetCommand(objectsPackage);
            IPackageViewUpdateCommand connectionCommand = null;
            switch (objectsPackage.ActionType)
            {
                case ActionType.Fuse:
                case ActionType.Tint:
                case ActionType.Remove:
                    connectionCommand = new UpdateConnectionsCommand(ConnectionsMap, (objectsPackage as ActionDataPackage<T>)
                        .Objects
                        .Select(o => Obj2Pt(Tint(o))));
                    break;
                case ActionType.Move:
                    connectionCommand = new UpdateConnectionsCommand(ConnectionsMap, (objectsPackage as ActionDataPackage<T>)
                        .Objects
                        .Select(o => Obj2Pt(Move(o))));
                    break;
                case ActionType.Connect:
                    connectionCommand = new AddCommand<SlamLine>(ConnectionsMap, (objectsPackage as ActionDataPackage<SlamLine>)
                        .Objects);
                    break;
                case ActionType.Clear:
                    connectionCommand = new UpdateCommand<SlamLine>(ConnectionsMap, ConnectionsMap.Select(ClearLines));
                    break;
                default:
                    connectionCommand = null;
                    break;
            }
            var commands = new List<IPackageViewUpdateCommand>(2);
            if (objectCommand != null)
                commands.Add(objectCommand);
            if (connectionCommand != null)
                commands.Add(connectionCommand);
            if (commands.Count == 0)
                return null;
            return new MacroCommand(commands);
        }
        public override IPackageViewUpdateCommand GetPostProcessCommand(ISlamActionPackage objectsPackage)
        {
            IPackageViewUpdateCommand objectCommand = base.GetPostProcessCommand(objectsPackage);
            IPackageViewUpdateCommand connectionCommand = null;
            switch (objectsPackage.ActionType)
            {
                case ActionType.Fuse:
                case ActionType.Tint:
                    connectionCommand = new UpdateConnectionsCommand(ConnectionsMap, 
                        (objectsPackage as ActionDataPackage<T>)
                            .Objects
                                .Select(o => Obj2Pt(PostProcessTint(o))));
                    break;
                case ActionType.Remove:
                    connectionCommand = new RemoveFromConnectionsCommand(ConnectionsMap, 
                        (objectsPackage as ActionDataPackage<T>)
                            .Objects
                                .Select(Obj2Pt));
                    break;
                case ActionType.Clear:
                    connectionCommand = new ClearCommand<SlamLine>(ConnectionsMap);
                    break;
                default:
                    connectionCommand = null;
                    break;
            }
            var commands = new List<IPackageViewUpdateCommand>(2);
            if (objectCommand != null)
                commands.Add(objectCommand);
            if (connectionCommand != null)
                commands.Add(connectionCommand);
            if (commands.Count == 0)
                return null;
            return new MacroCommand(commands);
        }
    }
}
