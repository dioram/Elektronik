using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamObservationsContainer : GameObjectsContainer<SlamObservation>
    {
        /// <param name="prefab">Desired prefab of observation</param>
        /// <param name="lines">Lines cloud objects for connections drawing</param>
        public SlamObservationsContainer(GameObject prefab) : base(prefab)
        {
        }

        protected override SlamPoint AsPoint(SlamObservation obj) => obj;

        protected override int GetObjectId(SlamObservation obj) => obj.point.id;

        protected override Pose GetObjectPose(SlamObservation obj) => new Pose(obj.point.position, obj.rotation);

        protected override SlamObservation Update(SlamObservation current, SlamObservation @new)
        {
            current.statistics = @new.statistics;
            current.point = @new.point;
            current.rotation = @new.rotation;
            current.message = @new.message;
            current.fileName = @new.fileName;
            return current;
        }

        protected override void UpdateGameObject(SlamObservation @object, GameObject gameObject)
            => gameObject.transform.SetPositionAndRotation(@object.point.position, @object.rotation);
    }
}
