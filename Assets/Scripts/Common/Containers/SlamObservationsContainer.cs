using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamObservationsContainer : GameObjectsContainer<SlamObservation>
    {
        #region GameObjetcsContainer implementation

        protected override SlamPoint AsPoint(SlamObservation obj) => obj;

        protected override int GetObjectId(SlamObservation obj) => obj.point.Id;

        protected override Pose GetObjectPose(SlamObservation obj) => new Pose(obj.point.position, obj.rotation);

        protected override SlamObservation UpdateItem(SlamObservation current, SlamObservation @new)
        {
            current.statistics = @new.statistics;
            current.point = @new.point;
            current.rotation = @new.rotation;
            current.message = @new.message;
            current.fileName = @new.fileName;
            return current;
        }

        protected override void UpdateGameObject(SlamObservation @object, GameObject gameObject)
        {
            gameObject.transform.SetPositionAndRotation(@object.point.position, @object.rotation);
        }

        #endregion
    }
}
