using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Commands.Generic
{
    public class RemoveTrackedObjCommands : RemoveCommand<TrackedObjPb>
    {
        private Dictionary<int, IList<SlamLine>> m_trackStates;

        private GameObjectsContainer<TrackedObjPb> m_goContainer;

        public RemoveTrackedObjCommands(GameObjectsContainer<TrackedObjPb> container, IEnumerable<TrackedObjPb> data)
            : base(container, data)
        {
            m_goContainer = container;
            m_trackStates = new Dictionary<int, IList<SlamLine>>();
            for (int i = 0; i < m_objs2Remove.Count; ++i)
            {
                if (container.TryGet(m_objs2Remove[i], out GameObject gameObject))
                {
                    var helmet = gameObject.GetComponent<Helmet>();
                    m_trackStates[m_objs2Remove[i].Id] = helmet.GetTrackState();
                }
            }
        }

        public override void UnExecute()
        {
            base.UnExecute();
            foreach (var o in m_objs2Remove)
            {
                if (m_goContainer.TryGet(o, out GameObject gameObject))
                {
                    var helmet = gameObject.GetComponent<Helmet>();
                    if (m_trackStates.ContainsKey(o.Id))
                    {
                        helmet.RestoreTrackState(m_trackStates[o.Id]);
                    }
                }
            }
        }

    }
}
