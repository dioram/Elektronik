using Elektronik.Offline.Loggers;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Presenters;
using Elektronik.Common.Data;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Packages.SlamActionPackages;
using System.Linq;

namespace Elektronik.Offline.Presenters
{
    public class SlamPackageInfoPresenter : RepaintablePackagePresenter
    {
        public EventInfoBanner info;
        public SlamMap map;

        private ISlamActionPackage m_pkg;
        private SlamPoint[] m_objects;

        private IEnumerable<SlamPoint> pkg2pts(ISlamActionPackage pkg)
        {
            switch (pkg.ObjectType)
            {
                case ObjectType.Point:
                    return (pkg as ActionDataPackage<SlamPoint>).Objects
                        .Select(pt => 
                        {
                            var mapPt = map.PointsContainer[pt];
                            mapPt.message = pt.message;
                            return mapPt;
                        });
                case ObjectType.Observation:
                    return (pkg as ActionDataPackage<SlamObservation>).Objects
                        .Select(obs =>
                        {
                            SlamPoint mapPt = map.ObservationsContainer[obs];
                            mapPt.message = obs.Point.message;
                            return mapPt;
                        }); 
                case ObjectType.Line:
                    return (pkg as ActionDataPackage<SlamLine>).Objects
                        .Select(l =>
                        {
                            var mapLine = map.LinesContainer[l];
                            SlamPoint pt1 = mapLine.pt1;
                            pt1.message = l.pt1.message;
                            return pt1;
                        });
                default:
                    return null;
            }
        }
        public override void Present(IPackage package)
        {
            if (package.PackageType == PackageType.SLAMPackage)
            {
                if (!package.IsKey)
                    return;
                m_pkg = package as ISlamActionPackage;
                if (m_pkg.ActionType == ActionType.Message)
                {
                    m_objects = pkg2pts(m_pkg).ToArray();
                }
            }
            m_presenter?.Present(package);
            return;
        }
        public override void Clear() => info.Clear();
        public override void Repaint()
        {
            if (m_pkg.IsKey)
            {
                info.Clear();
                info.UpdateCommonInfo(m_pkg.ToString());
                if (m_pkg.ActionType == ActionType.Message)
                    info.UpdateExtraInfo(m_pkg.ObjectType.ToString(), m_objects);
            }
        }
    }
}
