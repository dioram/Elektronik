using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common
{
    public class TrackingMap : AMap
    {
        public Helmet helmetPrefab;
        public ObjectPool m_helmetsPool;
        public IList<Helmet> m_helmets;
        private void Awake()
        {
            m_helmets = new List<Helmet>();
            m_helmetsPool = new ObjectPool(helmetPrefab.gameObject);
        }
        public override void Clear()
        {
            foreach (var helmet in m_helmets)
            {
                helmet.ResetHelmet();
                m_helmetsPool.Despawn(helmet.gameObject);
            }
            m_helmets.Clear();
        }
        public override void Repaint()
        {
        }

    }
}
