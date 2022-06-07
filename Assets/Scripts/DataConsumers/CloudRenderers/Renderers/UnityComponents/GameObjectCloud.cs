using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base implementation for renderer that renders items as GameObjects on scene. </summary>
    /// <typeparam name="TCloudItem"> Type of items it can render. </typeparam>
    internal abstract class GameObjectCloud<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        #region Editor fields

        [SerializeField] private GameObject ItemPrefab;
        
        #endregion

        #region ICloudRenderer
        
        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    UniRxExtensions.StartOnMainThread(() => AddInMainThread(sender, obj)).Subscribe();
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.UpdatedItems)
                {
                    UniRxExtensions.StartOnMainThread(() => UpdateInMainThread(sender, obj)).Subscribe();
                }
            }
        }

        /// <inheritdoc />
        public virtual void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            // First look how elektronik will work with rx.
            // TODO: Rewrite containers to use rx or this function back to simple loop
            e.RemovedItems.ToObservable()
                    .Select(item => (sender.GetHashCode(), item.Id))
                    .Select(PopItem)
                    .Where(go => go != null)
                    .ObserveOnMainThreadSafe()
                    .Do(RemoveInMainThread)
                    .Subscribe();
        }

        /// <inheritdoc />
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

        #region Protected
        
        protected ObjectPool ObjectsPool;

        /// <summary> Active GameObjects by their sender.GetHashID and TCloudItem.Id </summary>
        protected readonly Dictionary<(int, int), GameObject> GameObjects = new Dictionary<(int, int), GameObject>();

        /// <summary> Checks is object that sends update event is visible. </summary>
        /// <param name="sender"></param>
        protected static bool IsSenderVisible(object sender) => (sender as IVisibleDataSource)?.IsVisible ?? true;

        /// <summary> Gets pose from specific type of item. </summary>
        protected abstract Pose GetObjectPose(TCloudItem obj);

        /// <summary> Instantiates object on scene. Should be called from main thread. </summary>
        /// <param name="sender"> Object that sent item. </param>
        /// <param name="item"> Item that need to be rendered. </param>
        /// <returns> Instantiated GameObject. </returns>
        protected virtual GameObject AddInMainThread(object sender, TCloudItem item)
        {
            var pose = GetObjectPose(item);
            var go = ObjectsPool.Spawn(pose.position * _scale, pose.rotation);
            lock (GameObjects)
            {
                GameObjects[(sender.GetHashCode(), item.Id)] = go;
            }
            go.GetComponent<MeshRenderer>().material.SetColor(ColorProperty, item.ToPoint().Color);

            var dc = go.GetComponent(DataComponent<TCloudItem>.GetInstantiable());
            if (dc == null) dc = go.AddComponent(DataComponent<TCloudItem>.GetInstantiable());
            var dataComponent = (DataComponent<TCloudItem>) dc;
            dataComponent.Data = item;
            dataComponent.CloudContainer = sender as ICloudContainer<TCloudItem>;
            return go;
        }
        
        /// <summary> Updates object on scene. Should be called from main thread. </summary>
        /// <param name="sender"> Object that sent item. </param>
        /// <param name="item"> Item that need to be rendered. </param>
        /// <returns> Updated GameObject. </returns>
        protected virtual GameObject UpdateInMainThread(object sender, TCloudItem item)
        {
            var pose = GetObjectPose(item);
            var go = GameObjects[(sender.GetHashCode(), item.Id)];
            go.transform.SetPositionAndRotation(pose.position * _scale, pose.rotation);
            go.GetComponent<DataComponent<TCloudItem>>().Data = item;
            go.GetComponent<MeshRenderer>().material.SetColor(ColorProperty, item.ToPoint().Color);
            return go;
        }

        /// <summary> Removes object from scene. Should be called from main thread. </summary>
        /// <param name="go"> Object that needs to be removed. </param>
        protected virtual void RemoveInMainThread(GameObject go)
        {
            ObjectsPool.Despawn(go);
        }

        #endregion

        #region Private

        private float _scale = 1;
        
        // ReSharper disable once StaticMemberInGenericType
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        
        private void Clear()
        {
            lock (GameObjects)
            {
                GameObjects.Clear();
                UniRxExtensions.StartOnMainThread(() => ObjectsPool?.DespawnAllActiveObjects()).Subscribe();
            }
        }

        [CanBeNull] private GameObject PopItem((int, int) key)
        {
            lock (GameObjects)
            {
                if (!GameObjects.ContainsKey(key)) return null;
                var res = GameObjects[key];
                GameObjects.Remove(key);
                return res;
            }
        }

        #endregion
    }
}