using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Offline.Commanders.Slam
{
    public class ObservationsCommander : ConnectableObjectsCommander<SlamObservation>
    {
        public SlamMap slamMap;
        public override IConnectionsContainer<SlamLine> ConnectionsMap => slamMap.ObservationsConnections;
        public override IRepaintableContainer<SlamObservation> ObjectsMap => slamMap.ObservationsContainer;
        public override void GetCommands(IPackage pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.PackageType == PackageType.SLAMPackage)
            {
                var slamPkg = pkg as ISlamActionPackage;
                if (slamPkg.ObjectType == ObjectType.Observation)
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
        public override SlamObservation Move(SlamObservation obj)
        {
            SlamObservation current = ObjectsMap[obj];
            SlamPoint currentPt = current;
            currentPt.position = obj.Point.position;
            current.Orientation = obj.Orientation;
            current.Point = currentPt;
            return current;
        }
        public override SlamPoint Obj2Pt(SlamObservation obj) => obj;
        public override SlamObservation PostProcessTint(SlamObservation obj)
        {
            SlamObservation current = ObjectsMap[obj];
            SlamPoint currentPt = current;
            currentPt.color = currentPt.defaultColor;
            current.Point = currentPt;
            return current;
        }
        public override SlamObservation Tint(SlamObservation obj)
        {
            SlamObservation current = ObjectsMap[obj];
            SlamPoint currentPt = current;
            currentPt.color = obj.Point.color;
            current.Point = currentPt;
            return current;
        }
        public override SlamObservation ClearTint(SlamObservation obj)
        {
            SlamPoint currentPt = obj;
            currentPt.color = Color.red;
            obj.Point = currentPt;
            return base.ClearTint(obj);
        }
    }
}
