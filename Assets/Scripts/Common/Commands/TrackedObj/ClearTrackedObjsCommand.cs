﻿using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Commands.Generic
{
    public class ClearTrackedObjsCommand : ClearCommand<SlamTrackedObject>
    {
        private Dictionary<int, IList<SlamLine>> m_trackStates;

        private GameObjectsContainer<SlamTrackedObject> m_goContainer;

        public ClearTrackedObjsCommand(GameObjectsContainer<SlamTrackedObject> container)
            : base(container)
        {
            m_goContainer = container;
            m_trackStates = new Dictionary<int, IList<SlamLine>>();
            foreach (var o in container)
            {
                if (container.TryGet(o, out GameObject helmetGO))
                {
                    var helmet = helmetGO.GetComponent<Helmet>();
                    m_trackStates[o.Id] = helmet.GetTrackState();
                }
            }
        }

        public override void UnExecute()
        {
            base.UnExecute();
            foreach (var o in UndoObjects)
            {
                if (m_goContainer.TryGet(o, out GameObject helmetGO))
                {
                    var helmet = helmetGO.GetComponent<Helmet>();
                    if (m_trackStates.ContainsKey(o.Id))
                        helmet.RestoreTrackState(m_trackStates[o.Id]);
                }
            }
        }
    }
}
