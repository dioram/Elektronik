using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class TrackedObjectsContainer : GameObjectsContainer<SlamTrackedObject>
    {
        #region GameObjectsContainer implementation

        protected override int GetObjectId(SlamTrackedObject obj) => obj.id;

        protected override Pose GetObjectPose(SlamTrackedObject obj) => new Pose(obj.position, obj.rotation);

        protected override void UpdateGameObject(SlamTrackedObject @object, GameObject gameObject)
        {
            Helmet helmet = gameObject.GetComponent<Helmet>();
            helmet.color = @object.color;
            helmet.transform.SetPositionAndRotation(@object.position, @object.rotation);
        }

        protected override SlamTrackedObject UpdateItem(SlamTrackedObject current, SlamTrackedObject @new) => @new;

        protected override SlamPoint AsPoint(SlamTrackedObject obj)
            => new SlamPoint()
            {
                    color = obj.color,
                    id = obj.id,
                    position = obj.position,
            };

        #endregion
    }
}
