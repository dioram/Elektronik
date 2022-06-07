using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using TMPro;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for tracked objects. </summary>
    internal class TrackedObjectCloud : GameObjectCloud<SlamTrackedObject>
    {
        // TODO: Move Follow functions to separate class
        public void FollowCamera(IFollowableDataSource<SlamTrackedObject> sender, ICloudContainer<SlamTrackedObject> cloudContainer,
                                 SlamTrackedObject obj)
        {
            var cam = Camera.main;
            if (cam == null) return;
            var cameraTransform = cam.transform!;
            (cameraTransform.parent
                            .GetComponent<DataComponent<SlamTrackedObject>>()?
                            .CloudContainer)?.Children
                    .OfType<IFollowableDataSource<SlamTrackedObject>>()
                    .FirstOrDefault(f => f != sender && f.IsFollowed)?
                    .Unfollow();
            cameraTransform.parent = GameObjects[(cloudContainer.GetHashCode(), obj.Id)].transform;
        }

        public void StopFollowCamera(ICloudContainer<SlamTrackedObject> cloudContainer, SlamTrackedObject obj)
        {
            if (Camera.main.transform.parent == GameObjects[(cloudContainer.GetHashCode(), obj.Id)].transform)
            {
                Camera.main.transform.parent = null;
            }
        }
        
        #region Protected

        /// <inheritdoc />
        protected override GameObject AddInMainThread(object sender, SlamTrackedObject item)
        {
            var go = base.AddInMainThread(sender, item);
            go.transform.Find("Label").GetComponent<TMP_Text>().text =
                    $"{(sender as TrackedCloudObjectsContainer)?.ObjectLabel} #{item.Id}";
            return go;
        }

        /// <inheritdoc />
        protected override Pose GetObjectPose(SlamTrackedObject obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        #endregion
    }
}