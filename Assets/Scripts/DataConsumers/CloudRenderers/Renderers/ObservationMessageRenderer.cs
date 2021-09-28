using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Threading;
using TMPro;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ObservationMessageRenderer : GameObjectCloud<SlamObservation>
    {
        public override void OnItemsAdded(object sender, AddedEventArgs<SlamObservation> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        var go = ObjectsPool.Spawn(pose.position, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)] = go;

                        var dc = go.GetComponent(DataComponent<SlamObservation>.GetInstantiable());
                        if (dc == null) dc = go.AddComponent(DataComponent<SlamObservation>.GetInstantiable());
                        var dataComponent = (DataComponent<SlamObservation>) dc;
                        dataComponent.Data = obj;
                        dataComponent.Container = sender as IContainer<SlamObservation>;
                        go.transform.Find("Label").GetComponent<TMP_Text>().text = $"{obj.Message}";
                        go.SetActive(!string.IsNullOrEmpty(obj.Message) && gameObject.activeInHierarchy);
                    });
                }
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamObservation> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.UpdatedItems)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        var go = GameObjects[(sender.GetHashCode(), obj.Id)];
                        go.transform.SetPositionAndRotation(pose.position, pose.rotation);
                        go.GetComponent<DataComponent<SlamObservation>>().Data = obj;
                        go.transform.Find("Label").GetComponent<TMP_Text>().text = $"{obj.Message}";
                        go.SetActive(!string.IsNullOrEmpty(obj.Message) && gameObject.activeInHierarchy);
                    });
                }
            }
        }

        protected override Pose GetObjectPose(SlamObservation obj)
        {
            return new Pose(obj.Point.Position, obj.Rotation);
        }
    }
}