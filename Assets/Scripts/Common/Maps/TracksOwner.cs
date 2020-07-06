using System.Collections.Generic;

namespace Elektronik.Common.Maps
{
    public class TracksOwner : RepaintableObject
    {
        public Helmet helmetPrefab;

        public ObjectPool HelmetsPool { get; private set; }
        public IList<Helmet> Helmets { get; private set; }
        private void Awake()
        {
            Helmets = new List<Helmet>();
            HelmetsPool = new ObjectPool(helmetPrefab.gameObject);
        }
        public override void Clear()
        {
            foreach (var helmet in Helmets)
            {
                HelmetsPool.Despawn(helmet.gameObject);
            }
            Helmets.Clear();
        }
        public override void Repaint()
        {
        }

    }
}
