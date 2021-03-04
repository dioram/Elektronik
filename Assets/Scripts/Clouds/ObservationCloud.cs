using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class ObservationCloud : GameObjectCloud<SlamObservation>
    {
        protected override Pose GetObjectPose(SlamObservation obj)
        {
            return new Pose(obj.Point.Position, obj.Rotation);
        }
    }
}