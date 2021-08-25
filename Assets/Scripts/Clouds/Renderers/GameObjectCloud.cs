using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.Clouds
{
    public abstract class GameObjectCloud<TCloudItem> : CloudRendererComponent<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        public GameObject ItemPrefab;

        public override int ItemsCount
        {
            get
            {
                lock (GameObjects)
                {
                    return GameObjects.Count;
                }
            }
        }

        public override void SetScale(float value)
        {
            lock (GameObjects)
            {
                var factor = value / _scale;
                _scale = value;
                foreach (var go in GameObjects.Values)
                {
                    go.transform.position *= factor;
                }
            }
        }

        public void Clear()
        {
            lock (GameObjects)
            {
                GameObjects.Clear();
                if (MainThreadInvoker.Instance != null)
                {
                    MainThreadInvoker.Enqueue(() => ObservationsPool?.DespawnAllActiveObjects());
                }
            }
        }

        public List<DataComponent<TCloudItem>> GetObjects()
        {
            lock (GameObjects)
            {
                return GameObjects.Select(pair => pair.Value.GetComponent<DataComponent<TCloudItem>>()).ToList();
            }
        }

        #region ICloudRenderer implementaion

        public override void ShowItems(object sender, IEnumerable<TCloudItem> items)
        {
            if (!IsSenderVisible(sender)) return;
            OnClear(sender);
            lock (GameObjects)
            {
                foreach (var obj in items)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        var go = ObservationsPool.Spawn(pose.position * _scale, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)] = go;
                        go.GetComponent<MeshRenderer>().material.SetColor(EmissionColor, obj.AsPoint().Color);

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
                    MainThreadInvoker.Enqueue(() => ObservationsPool.Despawn(go));
                    GameObjects.Remove(key);
                }
            }
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    Pose pose = GetObjectPose(obj);
                    MainThreadInvoker.Enqueue(() =>
                    {
                        var go = ObservationsPool.Spawn(pose.position * _scale, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)] = go;
                        go.GetComponent<MeshRenderer>().material.SetColor(EmissionColor, obj.AsPoint().Color);

                        var dc = go.GetComponent(DataComponent<TCloudItem>.GetInstantiable());
                        if (dc == null) dc = go.AddComponent(DataComponent<TCloudItem>.GetInstantiable());
                        var dataComponent = (DataComponent<TCloudItem>) dc;
                        dataComponent.Data = obj;
                        dataComponent.Container = sender as IContainer<TCloudItem>;
                    });
                }
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
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
                        go.transform.SetPositionAndRotation(pose.position * _scale, pose.rotation);
                        go.GetComponent<DataComponent<TCloudItem>>().Data = obj;
                        go.GetComponent<MeshRenderer>().material.SetColor(EmissionColor, obj.AsPoint().Color);
                    });
                }
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs e)
        {
            lock (GameObjects)
            {
                foreach (var id in e.RemovedIds)
                {
                    var key = (sender.GetHashCode(), id);
                    if (!GameObjects.ContainsKey(key)) continue;

                    var go = GameObjects[key];
                    MainThreadInvoker.Enqueue(() =>
                    {
                        
                        ObservationsPool.Despawn(go);
                        GameObjects.Remove(key);
                    });
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
        // ReSharper disable once StaticMemberInGenericType
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        protected abstract Pose GetObjectPose(TCloudItem obj);

        #endregion

        #region Private

        private float _scale = 1;

        #endregion
    }
}