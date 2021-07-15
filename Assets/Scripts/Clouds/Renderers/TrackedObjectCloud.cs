using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using TMPro;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class TrackedObjectCloud : GameObjectCloud<SlamTrackedObject>
    {
        public override void OnItemsAdded(object sender, AddedEventArgs<SlamTrackedObject> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        var go = ObservationsPool.Spawn(pose.position, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)] = go;

                        var dc = go.GetComponent(DataComponent<SlamTrackedObject>.GetInstantiable());
                        if (dc == null) dc = go.AddComponent(DataComponent<SlamTrackedObject>.GetInstantiable());
                        var dataComponent = (DataComponent<SlamTrackedObject>) dc;
                        dataComponent.Data = obj;
                        dataComponent.Container = sender as IContainer<SlamTrackedObject>;

                        go.transform.Find("Label").GetComponent<TMP_Text>().text =
                                $"{(sender as TrackedObjectsContainer)?.ObjectLabel} #{obj.Id}";
                    });
                }
            }
        }

        protected override Pose GetObjectPose(SlamTrackedObject obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        public void FollowCamera(IFollowable<SlamTrackedObject> sender, IContainer<SlamTrackedObject> container,
                                 SlamTrackedObject obj)
        {
            (Camera.main.transform.parent?
                            .GetComponent<DataComponent<SlamTrackedObject>>()?
                            .Container as ISourceTree)?.Children
                    .OfType<IFollowable<SlamTrackedObject>>()
                    .FirstOrDefault(f => f != sender && f.IsFollowed)?
                    .Unfollow();
            Camera.main.transform.parent = GameObjects[(container.GetHashCode(), obj.Id)].transform;
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