using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Maps;
using Elektronik.Common.Presenters;
using System;

namespace Elektronik.Online.Presenters.Slam
{
    public class PointsPresenter : SlamObjectsPresenter<SlamPoint>
    {
        public SlamMap map;

        public override IRepaintableContainer<SlamPoint> Map => map.PointsContainer;

        public override void Clear()
        {
            map.PointsContainer.Clear();
            map.PointsConnections.Clear();
        }
        public override void Present(IPackage package)
        {
            if (package.PackageType == PackageType.SLAMPackage)
            {
                var slamPackage = package as ISlamActionPackage;
                if (slamPackage.ObjectType == ObjectType.Point)
                {
                    if (slamPackage is ActionDataPackage<SlamPoint>)
                        UpdateObjects(slamPackage as ActionDataPackage<SlamPoint>);
                    UpdateConnections(slamPackage);
                }
            }
            m_presenter?.Present(package);
        }
        public override void Repaint()
        {
            map.PointsContainer.Repaint();
            map.PointsConnections.Repaint();
        }

        protected override SlamPoint Move(SlamPoint p)
        {
            var pt = map.PointsContainer[p];
            pt.position = p.position;
            return pt;
        }

        protected override SlamPoint Tint(SlamPoint p)
        {
            var pt = map.PointsContainer[p];
            pt.color = p.color;
            return pt;
        }

        private void UpdateConnections(ISlamActionPackage package)
        {
            switch (package.ActionType)
            {
                case ActionType.Move:
                    var movedPts = package as ActionDataPackage<SlamPoint>;
                    map.PointsConnections.Update(movedPts.Objects);
                    break;
                case ActionType.Remove:
                    var removedPts = package as ActionDataPackage<SlamPoint>;
                    foreach (var pt in removedPts.Objects)
                        map.PointsConnections.Remove(pt.id);
                    break;
                case ActionType.Connect:
                    var connections = package as ActionDataPackage<SlamLine2>;
                    map.PointsConnections.Add(connections.Objects);
                    break;
                case ActionType.Clear:
                    map.PointsConnections.Clear();
                    break;
            }
        }
    }
}
