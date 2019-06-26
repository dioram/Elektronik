using Elektronik.Common;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Online
{
    public class SlamPackagePresenter : RepaintablePackagePresenter
    {
        public Map map;

        public override void Present(IPackage package)
        {
            if (package.Type != PackageType.SLAMPackage)
            {
                m_presenter?.Present(package);
                return;
            }
            SlamPackage slamPkg = package as SlamPackage;
            UpdateMaps(slamPkg);
            PostProcessMaps(slamPkg);
        }

        private void UpdateMaps(SlamPackage pkg)
        {
            lock (map.PointsContainer)
                UpdateMap(pkg.Points, p => p.isNew, p => p.isRemoved, p => p.justColored, p => p.id != -1, map.PointsContainer);
            lock (map.LinesContainer)
                UpdateMap(
                      pkg.Lines,
                      /*isNew*/ _ => true, /*isRemoved*/ _ => false, /*justColored*/ _ => false, /*isValid*/ _ => true,
                      map.LinesContainer);
            lock (map.ObservationsContainer)
                UpdateMap(
                      pkg.Observations,
                      o => o.Point.isNew, o => o.Point.isRemoved, o => o.Point.justColored, o => o.Point.id != -1,
                      map.ObservationsContainer);
        }
        private void PostProcessMaps(SlamPackage pkg)
        {
            if (pkg.Points != null)
            {
                SlamPoint[] updatedPoints = pkg.Points
                    .AsParallel()
                    .Where(p => p.id != -1)
                    .Where(p => !p.isRemoved)
                    .Select(p => { var mp = map.PointsContainer[p]; mp.color = mp.defaultColor; return mp; })
                    .ToArray();
                lock (map.PointsContainer)
                    UpdateMap(
                      updatedPoints,
                      /*isNew*/ _ => false, /*isRemoved*/ _ => false, /*justColored*/ _ => true, /*isValid*/ p => p.id != -1,
                      map.PointsContainer);
            }
            if (pkg.Lines != null)
            {
                lock (map.LinesContainer)
                    UpdateMap(
                      pkg.Lines,
                      /*isNew*/ _ => false, /*isRemoved*/ _ => true, /*justColored*/ _ => false, /*isValid*/ _ => true,
                      map.LinesContainer);
            }
        }

        private void UpdateMap<T>(
            ICollection<T> source,
            Func<T, bool> isNewSelector,
            Func<T, bool> isRemovedSelector,
            Func<T, bool> justColoredSelector,
            Func<T, bool> isValidSelector,
            ICloudObjectsContainer<T> map)
        {
            if (source != null)
            {
                foreach (var element in source)
                {
                    if (isValidSelector(element))
                    {
                        if (isNewSelector(element))
                            map.Add(element);
                        else if (isRemovedSelector(element))
                            map.Remove(element);
                        else if (justColoredSelector(element))
                            map.ChangeColor(element);
                        else
                            map.Update(element);
                    }
                }
            }
        }

        public override void Repaint()
        {
            Debug.Log(map == null);
            Debug.Log(map.PointsContainer == null);
            Debug.Log(map.LinesContainer == null);
            Debug.Log(map.ObservationsContainer == null);
            lock (map.PointsContainer) map.PointsContainer.Repaint();
            lock (map.LinesContainer) map.LinesContainer.Repaint();
            lock (map.ObservationsContainer) map.ObservationsContainer.Repaint();
        }

        public override void Clear()
        {
            Debug.Log(map == null);
            Debug.Log(map.PointsContainer == null);
            Debug.Log(map.LinesContainer == null);
            Debug.Log(map.ObservationsContainer == null);
            lock (map.PointsContainer) map.PointsContainer.Clear();
            lock (map.LinesContainer) map.LinesContainer.Clear();
            lock (map.ObservationsContainer) map.ObservationsContainer.Clear();
        }
    }
}
