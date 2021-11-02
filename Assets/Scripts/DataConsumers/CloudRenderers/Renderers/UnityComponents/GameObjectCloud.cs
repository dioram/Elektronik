using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public abstract class GameObjectCloud<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        #region Editor fields

        [SerializeField] private GameObject ItemPrefab;
        
        #endregion

        public int ItemsCount
        {
            get
            {
                lock (GameObjects)
                {
                    return GameObjects.Count;
                }
            }
        }
        
        public virtual float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;
                
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
        }

        public void Clear()
        {
            lock (GameObjects)
            {
                GameObjects.Clear();
                UniRxExtensions.StartOnMainThread(() => ObjectsPool?.DespawnAllActiveObjects()).Subscribe();
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

        public virtual void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    var pose = GetObjectPose(obj);
                    UniRxExtensions.StartOnMainThread(() => AddInMainThread(sender, obj, pose)).Subscribe();
                }
            }
        }

        public virtual void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.UpdatedItems)
                {
                    var pose = GetObjectPose(obj);
                    UniRxExtensions.StartOnMainThread(() => UpdateInMainThread(sender, obj, pose)).Subscribe();
                }
            }
        }

        public virtual void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            lock (GameObjects)
            {
                foreach (var item in e.RemovedItems)
                {
                    var key = (sender.GetHashCode(), item.Id);
                    if (!GameObjects.ContainsKey(key)) continue;

                    var go = GameObjects[key];
                    UniRxExtensions.StartOnMainThread(() => RemoveInMainThread(sender, item, go)).Subscribe();
                }
            }
        }

        public virtual void Dispose()
        {
            Clear();
        }

        #endregion

        #region Unity events

        private void Awake()
        {
            ObjectsPool = new ObjectPool(ItemPrefab);
        }

        protected virtual void OnEnable()
        {
            foreach (var o in GameObjects.Values)
            {
                o.SetActive(true);
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var o in GameObjects.Values.Where(o => o != null))
            {
                o.SetActive(false);
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

        protected ObjectPool ObjectsPool;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        
        protected static bool IsSenderVisible(object sender) => (sender as IVisible)?.IsVisible ?? true;

        protected abstract Pose GetObjectPose(TCloudItem obj);

        protected virtual GameObject AddInMainThread(object sender, TCloudItem item, Pose pose)
        {
            var go = ObjectsPool.Spawn(pose.position * _scale, pose.rotation);
            GameObjects[(sender.GetHashCode(), item.Id)] = go;
            go.GetComponent<MeshRenderer>().material.SetColor(ColorProperty, item.AsPoint().Color);

            var dc = go.GetComponent(DataComponent<TCloudItem>.GetInstantiable());
            if (dc == null) dc = go.AddComponent(DataComponent<TCloudItem>.GetInstantiable());
            var dataComponent = (DataComponent<TCloudItem>) dc;
            dataComponent.Data = item;
            dataComponent.Container = sender as IContainer<TCloudItem>;
            return go;
        }
        
        protected virtual GameObject UpdateInMainThread(object sender, TCloudItem item, Pose pose)
        {
            var go = GameObjects[(sender.GetHashCode(), item.Id)];
            go.transform.SetPositionAndRotation(pose.position * _scale, pose.rotation);
            go.GetComponent<DataComponent<TCloudItem>>().Data = item;
            go.GetComponent<MeshRenderer>().material.SetColor(ColorProperty, item.AsPoint().Color);
            return go;
        }

        protected virtual void RemoveInMainThread(object sender, TCloudItem item, GameObject go)
        {
            ObjectsPool.Despawn(go);
            GameObjects.Remove((sender.GetHashCode(), item.Id));
        }

        #endregion

        #region Private

        private float _scale = 1;

        #endregion
    }
}