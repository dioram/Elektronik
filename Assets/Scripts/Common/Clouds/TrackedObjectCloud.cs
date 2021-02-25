using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class TrackedObjectCloud : GameObjectCloud<SlamTrackedObject>
    {
        protected override Pose GetObjectPose(SlamTrackedObject obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }
    }
}