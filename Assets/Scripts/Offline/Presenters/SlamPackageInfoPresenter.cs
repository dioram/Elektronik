using Elektronik.Offline.Loggers;
using Elektronik.Common.Data.Packages;
using Elektronik.Common.Presenters;
using Elektronik.Common.Data;
using Elektronik.Common.Maps;

namespace Elektronik.Offline.Presenters
{
    public class SlamPackageInfoPresenter : RepaintablePackagePresenter
    {
        public EventLogger eventsLogger;
        public SlamMap map;

        private SlamPackage m_pkg2Present;

        public override void Present(IPackage package)
        {
            if (package.Type != PackageType.SLAMPackage)
            {
                m_presenter?.Present(package);
                return;
            }
            if (!package.IsKey)
                return;
            m_pkg2Present = package as SlamPackage;
        }

        public override void Clear()
        {
            eventsLogger.Clear();
        }

        public override void Repaint()
        {
            eventsLogger.UpdateInfo(m_pkg2Present, map.PointsContainer, map.ObservationsContainer);
        }
    }
}
