using Elektronik.Common;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Online
{
    public class TrackingPackagePresenter : PackagePresenter
    {
        public Helmet helmetPrefab;
        private IDictionary<int, Helmet> m_helmets;

        private void Awake()
        {
            m_helmets = new Dictionary<int, Helmet>();
        }

        public override void Present(IPackage package)
        {
            if (package.Type != PackageType.TrackingPackage)
            {
                m_presenter.Present(package);
                return;
            }
            var pkg = package as TrackingPackage;
            foreach (var track in pkg.Tracks)
            {
                if (!m_helmets.ContainsKey(track.id))
                {
                    m_helmets[track.id] = MF_AutoPool.Spawn(helmetPrefab.gameObject).GetComponent<Helmet>();
                }
                m_helmets[track.id].color = track.color;
                m_helmets[track.id].ReplaceAbs(track.position, track.rotation);
            }
            
        }
    }
}
