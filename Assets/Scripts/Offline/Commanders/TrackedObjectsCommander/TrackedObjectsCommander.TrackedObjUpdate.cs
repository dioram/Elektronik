using Elektronik.Common.Containers;
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
        private class TrackedObjUpdate : UpdateCommand<TrackedObjPb>
        {
            private GameObjectsContainer<TrackedObjPb> m_goContainer;

            public TrackedObjUpdate(GameObjectsContainer<TrackedObjPb> container, IEnumerable<TrackedObjPb> data)
                : base(container, data)
            {
                m_goContainer = container;
            }

            public override void Execute()
            {
                base.Execute();
                foreach (var o in m_objs2Update)
                {
                    if (m_goContainer.TryGet(o, out GameObject helmetGO))
                    {
                        helmetGO.GetComponent<Helmet>().IncrementTrack();
                    }
                }
            }

            public override void UnExecute()
            {
                base.UnExecute();
                foreach (var o in m_objs2Update)
                {
                    if (m_goContainer.TryGet(o, out GameObject helmetGO))
                    {
                        helmetGO.GetComponent<Helmet>().DecrementTrack();
                    }
                }
            }
        }
    }
}
