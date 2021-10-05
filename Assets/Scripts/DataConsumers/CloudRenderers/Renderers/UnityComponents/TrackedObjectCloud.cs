using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.SpecialInterfaces;
using TMPro;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class TrackedObjectCloud : GameObjectCloud<SlamTrackedObject>
    {
        protected override GameObject AddInMainThread(object sender, SlamTrackedObject item, Pose pose)
        {
            var go = base.AddInMainThread(sender, item, pose);
            go.transform.Find("Label").GetComponent<TMP_Text>().text =
                    $"{(sender as TrackedObjectsContainer)?.ObjectLabel} #{item.Id}";
            return go;
        }

        protected override Pose GetObjectPose(SlamTrackedObject obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        public void FollowCamera(IFollowable<SlamTrackedObject> sender, IContainer<SlamTrackedObject> container,
                                 SlamTrackedObject obj)
        {
            var cam = Camera.main;
            if (cam == null) return;
            var cameraTransform = cam.transform!;
            (cameraTransform.parent
                            .GetComponent<DataComponent<SlamTrackedObject>>()?
                            .Container as ISourceTreeNode)?.Children
                    .OfType<IFollowable<SlamTrackedObject>>()
                    .FirstOrDefault(f => f != sender && f.IsFollowed)?
                    .Unfollow();
            cameraTransform.parent = GameObjects[(container.GetHashCode(), obj.Id)].transform;
        }

        public void StopFollowCamera(IContainer<SlamTrackedObject> container, SlamTrackedObject obj)
        {
            if (Camera.main.transform.parent == GameObjects[(container.GetHashCode(), obj.Id)].transform)
            {
                Camera.main.transform.parent = null;
            }
        }
    }
}