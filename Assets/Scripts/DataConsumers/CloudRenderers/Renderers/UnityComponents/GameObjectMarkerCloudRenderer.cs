using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class GameObjectMarkerCloudRenderer : GameObjectCloud<SlamMarker>, IMarkerCloudRenderer
    {
        // ReSharper disable once InconsistentNaming
        [SerializeField] private SlamMarker.MarkerType _markerType;
        
        public SlamMarker.MarkerType MarkerType => _markerType;
        
        protected override Pose GetObjectPose(SlamMarker obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        protected override GameObject AddInMainThread(object sender, SlamMarker item, Pose pose)
        {
            var go = base.AddInMainThread(sender, item, pose);
            go.transform.localScale = item.Scale * Scale;
            return go;
        }

        protected override GameObject UpdateInMainThread(object sender, SlamMarker item, Pose pose)
        {
            var go = base.UpdateInMainThread(sender, item, pose);
            go.transform.localScale = item.Scale * Scale;
            return go;
        }
    }
}