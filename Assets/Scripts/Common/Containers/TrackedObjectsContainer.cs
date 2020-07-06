using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class TrackedObjectsContainer : GameObjectsContainer<TrackedObjPb>
    {
        public TrackedObjectsContainer(Helmet prefab, MainThreadInvoker invoker) : base(prefab.gameObject, invoker) { }

        protected override int GetObjectId(TrackedObjPb obj) => obj.Id;

        protected override Pose GetObjectPose(TrackedObjPb obj) => new Pose(obj.Translation, obj.Rotation);

        protected override void UpdateGameObject(TrackedObjPb @object, GameObject gameObject)
        {
            Helmet helmet = gameObject.GetComponent<Helmet>();
            helmet.color = @object.TrackColor;
            helmet.transform.SetPositionAndRotation(@object.Translation, @object.Rotation);
        }

        protected override TrackedObjPb Update(TrackedObjPb current, TrackedObjPb @new)
        {
            current = new TrackedObjPb(@new);
            return current;
        }

        protected override SlamPoint AsPoint(TrackedObjPb obj)
            => new SlamPoint()
            {
                color = Color.black,
                id = obj.Id,
                position = obj.Translation,
            };
    }
}
