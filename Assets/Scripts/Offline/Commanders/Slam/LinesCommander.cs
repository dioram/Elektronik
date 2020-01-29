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
    public class LinesCommander : ObjectsCommander<SlamLine>
    {
        public SlamMap slamMap;
        public override IRepaintableContainer<SlamLine> ObjectsMap => slamMap.LinesContainer;
        public override void GetCommands(IPackage pkg, in LinkedList<IPackageViewUpdateCommand> commands)
        {
            if (pkg.PackageType == PackageType.SLAMPackage)
            {
                var slamPkg = pkg as ISlamActionPackage;
                if (slamPkg.ObjectType == ObjectType.Line)
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
        public override SlamLine Move(SlamLine obj)
        {
            SlamLine current = ObjectsMap[obj];
            SlamPoint pt1 = current.pt1;
            SlamPoint pt2 = current.pt2;
            pt1.position = obj.pt1.position;
            pt2.position = obj.pt2.position;
            return new SlamLine(pt1, pt2);
        }
        public override SlamLine PostProcessTint(SlamLine obj)
        {
            SlamLine current = ObjectsMap[obj];
            SlamPoint pt1 = current.pt1;
            SlamPoint pt2 = current.pt2;
            pt1.color = pt1.defaultColor;
            pt2.color = pt2.defaultColor;
            return new SlamLine(pt1, pt2);
        }
        public override SlamLine Tint(SlamLine obj)
        {
            SlamLine current = ObjectsMap[obj];
            SlamPoint pt1 = current.pt1;
            SlamPoint pt2 = current.pt2;
            pt1.color = obj.pt1.color;
            pt2.color = obj.pt2.color;
            return new SlamLine(pt1, pt2);
        }
        public override SlamLine ClearTint(SlamLine obj)
        {
            SlamPoint pt1 = obj.pt1; pt1.color = Color.red;
            SlamPoint pt2 = obj.pt2; pt2.color = Color.red;
            return Tint(new SlamLine(pt1, pt2));
        }
    }
}
