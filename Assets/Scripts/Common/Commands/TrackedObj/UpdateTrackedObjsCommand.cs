using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Commands.Generic
{
    public class UpdateTrackedObjsCommand : UpdateCommand<SlamTrackedObject>
    {
        private GameObjectsContainer<SlamTrackedObject> m_goContainer;

        public UpdateTrackedObjsCommand(GameObjectsContainer<SlamTrackedObject> container, IEnumerable<SlamTrackedObject> data)
            : base(container, data)
        {
            m_goContainer = container;
        }

        public override void Execute()
        {
            base.Execute();
            for (int i = 0; i < Objs2Update.Count; ++i)
            {
                if (m_goContainer.TryGet(Objs2Update[i], out GameObject helmetGO))
                {
                    helmetGO.GetComponent<Helmet>().IncrementTrack();
                }
            }
        }

        public override void UnExecute()
        {
            base.UnExecute();
            foreach (var o in Objs2Update)
            {
                if (m_goContainer.TryGet(o, out GameObject helmetGO))
                {
                    helmetGO.GetComponent<Helmet>().DecrementTrack();
                }
            }
        }
    }
}
