using Elektronik.Common;
using Elektronik.Common.Data;
using System.Collections.Generic;

namespace Elektronik.Online
{
    public class TrackingPackagePresenter : RepaintablePackagePresenter
    {
        public Helmet helmetPrefab;
        private ObjectPool m_helmetsPool;
        private IDictionary<int, Helmet> m_helmets;

        private void Awake()
        {
            m_helmets = new Dictionary<int, Helmet>();
            m_helmetsPool = new ObjectPool(helmetPrefab.gameObject);
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
                    m_helmets[track.id] = m_helmetsPool.Spawn().GetComponent<Helmet>();
                }
                m_helmets[track.id].color = track.color;
                m_helmets[track.id].ReplaceAbs(track.position, track.rotation);
            }

        }

        public override void Repaint()
        {
        }

        public override void Clear()
        {
        }
    }
}
