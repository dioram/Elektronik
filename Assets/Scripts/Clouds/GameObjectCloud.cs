using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
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

        #region ICloudRenderer implementaion

        public override void ShowItems(object sender, IEnumerable<TCloudItem> items)
        {
            OnClear(sender);
            lock (GameObjects)
            {
                foreach (var obj in items)
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
                    });
                }
            }
        }

        public override void OnClear(object sender)
        {
            lock (GameObjects)
            {
                var keys = GameObjects.Keys.Where(k => k.Item1 == sender.GetHashCode()).ToList();
                foreach (var key in keys)
                {
                    if (!GameObjects.ContainsKey(key)) continue;

                    var go = GameObjects[key];
                    MainThreadInvoker.Instance.Enqueue(() => ObservationsPool.Despawn(go));
                    GameObjects.Remove(key);
                }
            }
        }

        public override void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e)
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

                        var dc = go.GetComponent(DataComponent<TCloudItem>.GetInstantiable());
                        if (dc == null) dc = go.AddComponent(DataComponent<TCloudItem>.GetInstantiable());
                        var dataComponent = (DataComponent<TCloudItem>) dc;
                        dataComponent.Data = obj;
                        dataComponent.Container = sender;
                    });
                }
            }
        }

        public override void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e)
        {
            lock (GameObjects)
            {
                foreach (var obj in e.UpdatedItems)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Instance.Enqueue(() =>
                    {
                        GameObjects[(sender.GetHashCode(), obj.Id)]
                                .transform
                                .SetPositionAndRotation(pose.position, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)].GetComponent<DataComponent<TCloudItem>>().Data =
                                obj;
                    });
                }
            }
        }

        public override void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e)
        {
            lock (GameObjects)
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
            foreach (var o in ObservationsPool.ActiveObject.Where(o => o != null))
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