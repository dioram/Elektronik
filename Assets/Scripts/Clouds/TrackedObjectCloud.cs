using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using TMPro;
using UnityEngine;

namespace Elektronik.Clouds
{
    public class TrackedObjectCloud : GameObjectCloud<SlamTrackedObject>
    {
        public override void OnItemsAdded(IContainer<SlamTrackedObject> sender, AddedEventArgs<SlamTrackedObject> e)
        {
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Instance.Enqueue(() =>
                    {
                        var go = ObservationsPool.Spawn(pose.position, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)] = go;

                        var dc = go.GetComponent(DataComponent<SlamTrackedObject>.GetInstantiable());
                        if (dc == null) dc = go.AddComponent(DataComponent<SlamTrackedObject>.GetInstantiable());
                        var dataComponent = (DataComponent<SlamTrackedObject>) dc;
                        dataComponent.Data = obj;
                        dataComponent.Container = sender;

                        go.transform.Find("Label").GetComponent<TMP_Text>().text =
                                $"{(sender as ISourceTree)?.DisplayName} #{obj.Id}";
                    });
                }
            }
        }

        protected override Pose GetObjectPose(SlamTrackedObject obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }
    }
}