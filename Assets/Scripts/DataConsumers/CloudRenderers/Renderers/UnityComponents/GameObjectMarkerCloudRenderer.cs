using Elektronik.DataObjects;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for markers that needs to be rendered as GameObject. </summary>
    internal class GameObjectMarkerCloudRenderer : GameObjectCloud<SlamMarker>, IMarkerCloudRenderer
    {
        #region Editor fields

        // ReSharper disable once InconsistentNaming
        [SerializeField] private SlamMarker.MarkerType _markerType;

        #endregion

        #region IMarkerCloudRenderer

        /// <inheritdoc />
        public SlamMarker.MarkerType MarkerType => _markerType;

        #endregion

        #region Protected
        
        /// <inheritdoc />
        protected override Pose GetObjectPose(SlamMarker obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        /// <inheritdoc />
        protected override GameObject AddInMainThread(object sender, SlamMarker item)
        {
            var go = base.AddInMainThread(sender, item);
            go.transform.localScale = item.Scale * Scale;
            return go;
        }

        /// <inheritdoc />
        protected override GameObject UpdateInMainThread(object sender, SlamMarker item)
        {
            var go = base.UpdateInMainThread(sender, item);
            go.transform.localScale = item.Scale * Scale;
            return go;
        }

        #endregion
    }
}