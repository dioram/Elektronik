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
        public SlamObservationsContainer(GameObject prefab, MainThreadInvoker invoker) : base(prefab, invoker)
        {
        }

        protected override SlamPoint AsPoint(SlamObservation obj) => obj;

        protected override int GetObjectId(SlamObservation obj) => obj.Point.id;

        protected override Pose GetObjectPose(SlamObservation obj) => new Pose(obj.Point.position, obj.Orientation);

        protected override SlamObservation Update(SlamObservation current, SlamObservation @new)
        {
            current.Statistics = @new.Statistics;
            current.Point = @new.Point;
            current.Orientation = @new.Orientation;
            return current;
        }

        protected override void UpdateGameObject(SlamObservation @object, GameObject gameObject)
            => gameObject.transform.SetPositionAndRotation(@object.Point.position, @object.Orientation);
    }
}
