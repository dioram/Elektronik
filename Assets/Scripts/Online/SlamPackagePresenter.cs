using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Online
{
    public class SlamPackagePresenter : RepaintablePackagePresenter
    {
        private ICloudObjectsContainer<SlamObservation> m_observationsContainer;
        private ICloudObjectsContainer<SlamLine> m_linesContainer;
        private ICloudObjectsContainer<SlamPoint> m_pointsContainer;

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
            lock (m_pointsContainer)
                UpdateMap(pkg.Points, p => p.isNew, p => p.isRemoved, p => p.justColored, p => p.id != -1, m_pointsContainer);
            lock (m_linesContainer)
                UpdateMap(
                      pkg.Lines,
                      /*isNew*/ _ => true, /*isRemoved*/ _ => false, /*justColored*/ _ => false, /*isValid*/ _ => true,
                      m_linesContainer);
            lock (m_observationsContainer)
                UpdateMap(
                      pkg.Observations,
                      o => o.Point.isNew, o => o.Point.isRemoved, o => o.Point.justColored, o => o.Point.id != -1,
                      m_observationsContainer);
        }
        private void PostProcessMaps(SlamPackage pkg)
        {
            if (pkg.Points != null)
            {
                SlamPoint[] updatedPoints = pkg.Points
                    .AsParallel()
                    .Where(p => p.id != -1)
                    .Where(p => !p.isRemoved)
                    .Select(p => { var mp = m_pointsContainer[p]; mp.color = mp.defaultColor; return mp; })
                    .ToArray();
                lock (m_pointsContainer)
                    UpdateMap(
                      updatedPoints,
                      /*isNew*/ _ => false, /*isRemoved*/ _ => false, /*justColored*/ _ => true, /*isValid*/ p => p.id != -1,
                      m_pointsContainer);
            }
            if (pkg.Lines != null)
            {
                lock (m_linesContainer)
                    UpdateMap(
                      pkg.Lines,
                      /*isNew*/ _ => false, /*isRemoved*/ _ => true, /*justColored*/ _ => false, /*isValid*/ _ => true,
                      m_linesContainer);
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
            lock (m_pointsContainer) m_pointsContainer.Repaint();
            lock (m_linesContainer) m_linesContainer.Repaint();
            lock (m_observationsContainer) m_observationsContainer.Repaint();
        }

        public override void Clear()
        {
            lock (m_pointsContainer) m_pointsContainer.Clear();
            lock (m_linesContainer) m_linesContainer.Clear();
            lock (m_observationsContainer) m_observationsContainer.Clear();
        }
    }
}
