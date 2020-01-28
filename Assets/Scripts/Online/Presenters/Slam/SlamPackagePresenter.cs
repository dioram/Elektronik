using Elektronik.Common.Presenters;
using Elektronik.Common.Maps;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Data.Packages.SlamActionPackages;

namespace Elektronik.Online.Presenters.Slam
{
    public class SlamPackagePresenter : RepaintablePackagePresenter
    {
        public SlamMap map;

        public override void Present(IPackage package)
        {
            if (package.PackageType == PackageType.SLAMPackage)
            {
                var slamPkg = package as ISlamActionPackage;
                UpdateMaps(slamPkg);
                Repaint();
                //PostProcessMaps(slamPkg);
            }
            m_presenter?.Present(package);
        }

        private void UpdateMaps(ISlamActionPackage pkg)
        {
            ISlamActionPackage slamPkg = pkg as ISlamActionPackage;
            switch (slamPkg.ObjectType)
            {
                case ObjectType.Point:
                    lock (map.PointsContainer)
                        UpdateMap((slamPkg as ActionDataPackage<SlamPoint>).Objects,
                            map.PointsContainer,
                            pkg.ActionType);
                    break;
                case ObjectType.Observation:
                    lock (map.ObservationsContainer)
                        UpdateMap(
                            (slamPkg as ActionDataPackage<SlamObservation>).Objects,
                            map.ObservationsContainer,
                            pkg.ActionType);
                    break;
                case ObjectType.Line:
                    lock (map.LinesContainer)
                        UpdateMap(
                            (slamPkg as ActionDataPackage<SlamLine2>).Objects,
                            map.LinesContainer,
                            pkg.ActionType);
                    break;
            }
        }
        private void PostProcessMaps(ISlamActionPackage pkg)
        {
            if (pkg.ActionType == ActionType.Message)
                return;
            switch (pkg.ObjectType)
            {
                case ObjectType.Point:
                    SlamPoint[] updatedPoints = (pkg as ActionDataPackage<SlamPoint>).Objects
                        .AsParallel()
                        .Select(p => { var mp = map.PointsContainer[p]; mp.color = mp.defaultColor; return mp; })
                        .ToArray();
                    lock (map.PointsContainer)
                        UpdateMap(
                          updatedPoints,
                          map.PointsContainer,
                          ActionType.Tint);
                    break;
                case ObjectType.Observation:
                    lock (map.ObservationsContainer)
                        UpdateMap(
                            (pkg as ActionDataPackage<SlamObservation>).Objects,
                            map.ObservationsContainer,
                            ActionType.Tint);
                    break;
                case ObjectType.Line:
                    lock (map.LinesContainer)
                        UpdateMap(
                          (pkg as ActionDataPackage<SlamLine2>).Objects,
                          map.LinesContainer,
                          ActionType.Tint);
                    break;
            }
        }

        private void UpdateMap<T>(
            ICollection<T> source,
            IContainer<T> map,
            ActionType actionType)
        {
            if (source != null)
            {
                if (actionType != ActionType.Message)
                {
                    foreach (var element in source)
                    {
                        switch (actionType)
                        {
                            case ActionType.Create:
                                map.Add(element);
                                break;
                            case ActionType.Fuse:
                            case ActionType.Tint:
                            case ActionType.Move:
                                map.Update(element);
                                break;
                            case ActionType.Remove:
                                map.Remove(element);
                                break;
                        }
                    }
                    if (actionType == ActionType.Clear)
                        map.Clear();
                }
            }
        }

        public override void Repaint()
        {
            lock (map) map.Repaint();
        }

        public override void Clear()
        {
            lock (map) map.Clear();
        }
    }
}
