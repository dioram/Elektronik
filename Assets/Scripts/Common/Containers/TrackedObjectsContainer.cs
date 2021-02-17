using Elektronik.Common.Data.PackageObjects;
using Elektronik.Common.Maps;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class TrackedObjectsContainer : GameObjectsContainer<SlamTrackedObject>
    {
        #region Unity events
        
        protected override void OnEnable()
        {
            foreach (var o in ObservationsPool.ActiveObject)
            {
                o.GetComponent<Helmet>().SetActive(true);
            }
        }

        protected override void OnDisable()
        {
            foreach (var o in ObservationsPool.ActiveObject)
            {
                o.GetComponent<Helmet>().SetActive(false);
            }
        }

        #endregion
        
        #region GameObjectsContainer implementation

        protected override int GetObjectId(SlamTrackedObject obj) => obj.Id;

        protected override Pose GetObjectPose(SlamTrackedObject obj) => new Pose(obj.Position, obj.Rotation);

        protected override void UpdateGameObject(SlamTrackedObject @object, GameObject gameObject)
        {
            Helmet helmet = gameObject.GetComponent<Helmet>();
            helmet.Color = @object.Color;
            helmet.transform.SetPositionAndRotation(@object.Position, @object.Rotation);
        }

        protected override SlamTrackedObject UpdateItem(SlamTrackedObject current, SlamTrackedObject @new) => @new;

        protected override SlamPoint AsPoint(SlamTrackedObject obj)
            => new SlamPoint()
            {
                    Color = obj.Color,
                    Id = obj.Id,
                    Position = obj.Position,
            };

        #endregion
    }
}
