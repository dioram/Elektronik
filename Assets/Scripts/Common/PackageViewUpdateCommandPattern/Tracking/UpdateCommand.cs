using Elektronik.Common.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.PackageViewUpdateCommandPattern.Tracking
{
    public class UpdateCommand : IPackageViewUpdateCommand
    {
        private TrackingPackage m_pkg;
        private IList<Helmet> m_helmets;
        private ObjectPool m_helmetsPool;
        private Stack<Helmet> m_addedHelmets;
        private Stack<Color> m_oldColors;


        public UpdateCommand(IList<Helmet> existingHelmets, TrackingPackage package, ObjectPool helmetsPool)
        {
            m_pkg = package;
            m_helmetsPool = helmetsPool;
            m_helmets = existingHelmets;
            m_addedHelmets = new Stack<Helmet>();
            m_oldColors = new Stack<Color>();
        }

        public void Execute()
        {
            foreach (var track in m_pkg.Tracks)
            {
                Helmet hmd = m_helmets.FirstOrDefault(h => h.id == track.id);
                if (hmd == null)
                {
                    hmd = m_helmetsPool.Spawn().GetComponent<Helmet>();
                    hmd.id = track.id;
                    m_helmets.Add(hmd);
                    m_addedHelmets.Push(hmd);
                }
                m_oldColors.Push(hmd.color);
                hmd.color = track.color;
                hmd.ReplaceAbs(track.position, track.rotation);
            }
        }
        public void UnExecute()
        {
            foreach (var track in m_pkg.Tracks)
            {
                Helmet hmd = m_helmets.First(h => h.id == track.id);
                hmd.TurnBack();
                hmd.color = m_oldColors.Pop();
                if (m_addedHelmets.Count > 0 && m_addedHelmets.Peek() == hmd)
                {
                    m_addedHelmets.Pop();
                    hmd.ResetHelmet();
                    m_helmets.Remove(hmd);
                    m_helmetsPool.Despawn(hmd.gameObject);
                }
            }
        }
    }
}
