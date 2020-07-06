using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using Elektronik.Common.PackageViewUpdateCommandPattern.Slam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Offline.Commanders.TrackedObjectsCommander
{
    public partial class TrackedObjectsCommander
    {
        private class TrackedObjRemove : RemoveCommand<TrackedObjPb>
        {
            private Dictionary<int, SlamLine[]> m_trackStates;

            private GameObjectsContainer<TrackedObjPb> m_goContainer;

            public TrackedObjRemove(GameObjectsContainer<TrackedObjPb> container, IEnumerable<TrackedObjPb> data)
                : base(container, data)
            {
                m_goContainer = container;
                m_trackStates = new Dictionary<int, SlamLine[]>();
                foreach (var o in data)
                {
                    if (container.TryGet(o, out GameObject gameObject))
                    {
                        var helmet = gameObject.GetComponent<Helmet>();
                        m_trackStates[o.Id] = helmet.GetTrackState();
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
}
