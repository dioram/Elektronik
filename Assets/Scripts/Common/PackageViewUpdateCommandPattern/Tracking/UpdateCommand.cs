using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Tracking
{
    public class UpdateCommand : IPackageViewUpdateCommand
    {
        private TrackingPackage m_pkg;

        private IDictionary<int, Helmet> m_helmets;
        private IDictionary<int, Color> m_oldColors;
        private IList<int> m_addedHelmets;

        public UpdateCommand(IList<Helmet> helmets, TrackingPackage package, Helmet helmetPrefab)
        {
            m_oldColors = new Dictionary<int, Color>();
            m_addedHelmets = new List<int>();
            foreach (var track in package.Tracks)
            {
                if (!m_helmets.ContainsKey(track.id))
                {
                    m_addedHelmets.Add(track.id);
                    helmets.Add(MF_AutoPool.Spawn(helmetPrefab.gameObject).GetComponent<Helmet>());
                }
                else
                {
                    m_oldColors[track.id] = m_helmets[track.id].color;
                }
            }
            m_pkg = package;
            m_helmets = helmets.ToDictionary(h => h.id);
        }

        public void Execute()
        {
            foreach (var track in m_pkg.Tracks)
            {
                m_helmets[track.id].color = track.color;
                m_helmets[track.id].ReplaceAbs(track.position, track.rotation);
            }
        }
        public void UnExecute()
        {
            foreach (var track in m_pkg.Tracks)
            {
                m_helmets[track.id].TurnBack();
                m_helmets[track.id].color = m_oldColors[track.id];
                if (m_addedHelmets.Contains(track.id))
                {
                    m_helmets[track.id].ResetHelmet();
                    MF_AutoPool.Despawn(m_helmets[track.id].gameObject);
                }
            }
        }
    }
}
