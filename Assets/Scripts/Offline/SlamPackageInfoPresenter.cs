using Elektronik.Common;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamPackageInfoPresenter : RepaintablePackagePresenter
    {
        public EventLogger eventsLogger;
        public Map map;

        private SlamPackage m_pkg2Present;

        public override void Present(IPackage package)
        {
            if (package.Type != PackageType.SLAMPackage)
            {
                m_presenter?.SetSuccessor(m_presenter);
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
