using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Offline.Commanders.Slam
{
    public class PointsCommander : ConnectableObjectsCommander<SlamPoint>
    {
        public SlamMap slamMap;
        public override void GetCommands(IPackage pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.PackageType == PackageType.SLAMPackage)
            {
                var slamPkg = pkg as ISlamActionPackage;
                if (slamPkg.ObjectType == ObjectType.Point)
                {
                    var cmd = GetCommand(slamPkg);
                    if (cmd != null)
                        commands.AddLast(cmd).Value.Execute();
                    cmd = GetPostProcessCommand(slamPkg);
                    if (cmd != null)
                        commands.AddLast(cmd).Value.Execute();
                }
            }
            base.GetCommands(pkg, commands);
        }
        public override IConnectionsContainer<SlamLine> ConnectionsMap => slamMap.PointsConnections;
        public override IRepaintableContainer<SlamPoint> ObjectsMap => slamMap.PointsContainer;
        public override SlamPoint Move(SlamPoint obj)
        {
            SlamPoint current = ObjectsMap[obj];
            current.position = obj.position;
            return current;
        }
        public override SlamPoint Obj2Pt(SlamPoint obj) => obj;
        public override SlamPoint PostProcessTint(SlamPoint obj)
        {
            SlamPoint current = ObjectsMap[obj];
            current.color = current.defaultColor;
            return current;
        }
        public override SlamPoint Tint(SlamPoint obj)
        {
            SlamPoint current = ObjectsMap[obj];
            current.color = obj.color;
            return current;
        }
        public override SlamPoint ClearTint(SlamPoint obj)
        {
            obj.color = Color.red;
            return Tint(obj);
        }
    }
}
