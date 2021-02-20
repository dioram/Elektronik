using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Commands.Generic
{
    public class RemoveTrackedObjCommands : RemoveCommand<SlamTrackedObject>
    {
        private Dictionary<int, IList<SlamLine>> m_trackStates;

        private GameObjectsContainer<SlamTrackedObject> m_goContainer;

        public RemoveTrackedObjCommands(GameObjectsContainer<SlamTrackedObject> container, IEnumerable<SlamTrackedObject> data)
            : base(container, data)
        {
            m_goContainer = container;
            m_trackStates = new Dictionary<int, IList<SlamLine>>();
            for (int i = 0; i < Objs2Remove.Count; ++i)
            {
                if (container.TryGet(Objs2Remove[i], out GameObject gameObject))
                {
                    var helmet = gameObject.GetComponent<Helmet>();
                    m_trackStates[Objs2Remove[i].Id] = helmet.GetTrackState();
                }
            }
        }

        public override void UnExecute()
        {
            base.UnExecute();
            foreach (var o in Objs2Remove)
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
