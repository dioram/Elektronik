using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class ObservationCloud : GameObjectCloud<SlamObservation>
    {
        protected override Pose GetObjectPose(SlamObservation obj)
        {
            return new Pose(obj.Point.Position, obj.Rotation);
        }
    }
}