using System.Collections.Generic;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public abstract class GameObjectCloud<TCloudItem> : CloudRendererComponent<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        public GameObject ItemPrefab;

        public void Clear()
        {
            GameObjects.Clear();
            if (MainThreadInvoker.Instance != null)
            {
                MainThreadInvoker.Instance.Enqueue(() => ObservationsPool?.DespawnAllActiveObjects());
            }
        }

        #region ICloud implementaion

        public override void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e)
        {
            foreach (var obj in e.AddedItems)
            {
                Pose pose = GetObjectPose(obj);
                MainThreadInvoker.Instance.Enqueue(() =>
                {
                    var go = ObservationsPool.Spawn(pose.position, pose.rotation);
                    GameObjects[(sender.GetHashCode(), obj.Id)] = go;

                    var dc = go.GetComponent(DataComponent<TCloudItem>.GetInstantiable());
                    if (dc == null) dc = go.AddComponent(DataComponent<TCloudItem>.GetInstantiable());
                    var dataComponent = (DataComponent<TCloudItem>) dc;
                    dataComponent.Data = obj;
                    dataComponent.Container = sender;
                });
            }
        }

        public override void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e)
        {
            foreach (var obj in e.UpdatedItems)
            {
                Pose pose = GetObjectPose(obj);
                MainThreadInvoker.Instance.Enqueue(() =>
                {
                    GameObjects[(sender.GetHashCode(), obj.Id)]
                            .transform
                            .SetPositionAndRotation(pose.position, pose.rotation);
                    GameObjects[(sender.GetHashCode(), obj.Id)].GetComponent<DataComponent<TCloudItem>>().Data = obj;
                });
            }
        }

        public override void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e)
        {
            foreach (var id in e.RemovedIds)
            {
                var key = (sender.GetHashCode(), id);
                if (!GameObjects.ContainsKey(key)) continue;
                
                var go = GameObjects[key];
                MainThreadInvoker.Instance.Enqueue(() => ObservationsPool.Despawn(go));
                GameObjects.Remove(key);
            }
        }

        #endregion

        #region Unity events

        private void Awake()
        {
            ObservationsPool = new ObjectPool(ItemPrefab);
        }

        protected virtual void OnEnable()
        {
            foreach (var o in ObservationsPool.ActiveObject)
            {
                var meshRenderer = o.GetComponent<MeshRenderer>();
                meshRenderer.enabled = true;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var o in ObservationsPool.ActiveObject)
            {
                var meshRenderer = o.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region Protected definitions

        protected readonly SortedDictionary<(int, int), GameObject> GameObjects =
                new SortedDictionary<(int, int), GameObject>();

        protected ObjectPool ObservationsPool;

        protected abstract Pose GetObjectPose(TCloudItem obj);

        #endregion
    }
}