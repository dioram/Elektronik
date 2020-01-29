using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using Elektronik.Common.Maps;
using System.Linq;

namespace Elektronik.Online.Presenters.Slam
{
    public class ObservationsPresenter : SlamObjectsPresenter<SlamObservation>
    {
        public SlamMap map;
        public override IRepaintableContainer<SlamObservation> Map => map.ObservationsContainer;
        public override void Present(IPackage package)
        {
            if (package.PackageType == PackageType.SLAMPackage)
            {
                var slamPackage = package as ISlamActionPackage;
                if (slamPackage.ObjectType == ObjectType.Observation)
                {
                    if (slamPackage is ActionDataPackage<SlamObservation>)
                        UpdateObjects(slamPackage as ActionDataPackage<SlamObservation>);
                    UpdateConnections(slamPackage);
                }
            }
            m_presenter?.Present(package);
        }
        public override void Repaint()
        {
            base.Repaint();
            map.ObservationsConnections.Repaint();
        }
        public override void Clear()
        {
            base.Clear();
            map.ObservationsConnections.Clear();
        }
        private void UpdateConnections(ISlamActionPackage package)
        {
            switch (package.ActionType)
            {
                case ActionType.Move:
                    var movedObs = package as ActionDataPackage<SlamObservation>;
                    map.ObservationsConnections.Update(movedObs.Objects.Select(o => o.Point));
                    break;
                case ActionType.Remove:
                    var removedObs = package as ActionDataPackage<SlamObservation>;
                    map.ObservationsConnections.Remove(removedObs.Objects.Select(o => o.Point.id));
                    break;
                case ActionType.Connect:
                    var connections = package as ActionDataPackage<SlamLine>;
                    map.ObservationsConnections.Add(connections.Objects);
                    break;
                case ActionType.Clear:
                    map.ObservationsConnections.Clear();
                    break;
            }
        }

        protected override SlamObservation Tint(SlamObservation p)
        {
            SlamObservation obs = map.ObservationsContainer[p];
            SlamPoint pt = obs;
            pt.color = p.Point.color;
            obs.Point = pt;
            return obs;
        }

        protected override SlamObservation Move(SlamObservation p)
        {
            SlamObservation obs = map.ObservationsContainer[p];
            SlamPoint pt = obs;
            pt.position = p.Point.position;
            obs.Orientation = p.Orientation;
            obs.Point = pt;
            return obs;
        }
    }
}
