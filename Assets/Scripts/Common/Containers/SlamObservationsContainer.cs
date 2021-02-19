using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamObservationsContainer : GameObjectsContainer<SlamObservation>
    {
        #region GameObjetcsContainer implementation

        protected override SlamPoint AsPoint(SlamObservation obj) => obj;

        protected override int GetObjectId(SlamObservation obj) => obj.Point.Id;

        protected override Pose GetObjectPose(SlamObservation obj) => new Pose(obj.Point.Position, obj.Rotation);

        protected override SlamObservation UpdateItem(SlamObservation current, SlamObservation @new)
        {
            current.Statistics = @new.Statistics;
            current.Point = @new.Point;
            current.Rotation = @new.Rotation;
            current.Message = @new.Message;
            current.FileName = @new.FileName;
            return current;
        }

        protected override void UpdateGameObject(SlamObservation @object, GameObject gameObject)
        {
            gameObject.transform.SetPositionAndRotation(@object.Point.Position, @object.Rotation);
        }

        #endregion
    }
}
