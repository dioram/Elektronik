using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using Elektronik.Common.Clouds;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public abstract class GameObjectsContainer<T> : MonoBehaviour, ICloudObjectsContainer<T> where T: ICloudItem
    {
        public GameObject ObservationPrefab;
        
        #region Unity events

        private void Awake()
        {
            ObservationsPool = new ObjectPool(ObservationPrefab);
            ObservationsPool.OnObjectDeSpawn += (o, s) => GameObjectDespawn(o.GetComponent<GameObject>());
        }

        #endregion
        
        #region IContainer implementation

        [Obsolete("Never raised for now.")]
        public event Action<IContainer<T>, IEnumerable<T>> ItemsAdded;
        
        [Obsolete("Never raised for now.")]
        public event Action<IContainer<T>, IEnumerable<T>> ItemsUpdated;
        
        [Obsolete("Never raised for now.")]
        public event Action<IContainer<T>, IEnumerable<int>> ItemsRemoved;

        public int Count => Objects.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns >Clone of SlamObservation from graph </returns>
        public T this[T obj]
        {
            get => this[GetObjectId(obj)];
            set => this[GetObjectId(obj)] = value;
        }
        
        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public T this[int id]
        {
            get => Objects[id];
            set
            {
                if (!TryGet(id, out _)) Add(value); else UpdateItem(value);
            }
        }

        public bool TryGet(T obj, out GameObject gameObject)
        {
            gameObject = null;
            if (Contains(GetObjectId(obj)))
            {
                gameObject = GameObjects[GetObjectId(obj)];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try get observation from graph
        /// </summary>
        /// <param name="obj">Slam observation you want to find</param>
        /// <param name="current">Clone of observation in graph or null if doesn't exist</param>
        /// <returns>Returns false if observation with given id from obj exists, otherwise true.</returns>
        public bool TryGet(T obj, out T current) => TryGet(GetObjectId(obj), out current);

        public IEnumerator<T> GetEnumerator() => Objects.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Objects.Values.GetEnumerator();

        public int IndexOf(T item) => GetObjectId(item);

        public void CopyTo(T[] array, int arrayIndex) => Objects.Values.CopyTo(array, arrayIndex);

        /// <summary>
        /// Check node existence by SlamObservation object
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Contains(T obj) => Contains(GetObjectId(obj));
        
        /// <summary>
        /// Add ref of observation into the graph. Make sure that you don't use it anywhere!
        /// Pass the clone of observation if you are not sure.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Id of observation</returns>
        public void Add(T obj)
        {
            int id = GetObjectId(obj);
            Objects[id] = obj;

            Pose pose = GetObjectPose(obj);
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                var go = ObservationsPool.Spawn(pose.position, pose.rotation);
                GameObjects[GetObjectId(obj)] = go;

                var idc = go.GetComponent<IdContainer>();
                if (idc == null) idc = go.AddComponent<IdContainer>();
                idc.Id = id;
            });
            Debug.Log($"[GameObjectsContainer.Add] Added {typeof(T).Name} with id {id}");
        }

        public void Insert(int index, T item) => Add(item);

        /// <summary>
        /// Look at the summary of Add
        /// </summary>
        /// <param name="objects"></param>
        public void AddRange(IEnumerable<T> objects)
        {
            foreach (var obj in objects)
                Add(obj);
        }
        
        /// <summary>
        /// Copy data from obj.
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateItem(T obj)
        {
            Objects[GetObjectId(obj)] = UpdateItem(Objects[GetObjectId(obj)], obj);
            var object2Update = GameObjects[GetObjectId(obj)];
            MainThreadInvoker.Instance.Enqueue(() => UpdateGameObject(obj, object2Update));
            Debug.Log($"[GameObjectsContainer.Update] Updated {typeof(T).Name} with id {GetObjectId(obj)}");
        }

        public void UpdateItems(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
                UpdateItem(obj);
        }
        
        /// <summary>
        /// Remove by id
        /// </summary>
        /// <param name="id"></param>
        public bool Remove(int id)
        {
            if (GameObjects.TryGetValue(id, out GameObject object2Remove))
            {
                MainThreadInvoker.Instance.Enqueue(() => ObservationsPool.Despawn(object2Remove));
                GameObjects.Remove(id);
                Objects.Remove(id);
                Debug.Log($"[GameObjectsContainer.Remove] Removing {typeof(T).Name} with id {id}");
                return true;
            }
            return false;
        }

        public void RemoveAt(int id) => Remove(id);

        /// <summary>
        /// Remove by SlamObservation object
        /// </summary>
        /// <param name="obj"></param>
        public bool Remove(T obj) => Remove(GetObjectId(obj));

        public void Remove(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
                Remove(obj);
        }

        /// <summary>
        /// Clear graph
        /// </summary>
        public void Clear()
        {
            GameObjects.Clear();
            Objects.Clear();
            MainThreadInvoker.Instance.Enqueue(() => ObservationsPool.DespawnAllActiveObjects());
            Debug.Log($"[GameObjectsContainer.Clear]");
        }

        #endregion

        #region ICloudObjectsContainer implementation
        
        public bool Contains(int objId) => Objects.ContainsKey(objId);
        
        public bool TryGet(int idx, out T current) => Objects.TryGetValue(idx, out current);
        
        public bool TryGetAsPoint(T obj, out SlamPoint point) => TryGetAsPoint(GetObjectId(obj), out point);

        public bool TryGetAsPoint(int idx, out SlamPoint point)
        {
            point = new SlamPoint();
            if (TryGet(idx, out var result))
            {
                point = AsPoint(result);
                return true;
            }
            return false;
        }
        
        #endregion

        #region Protected definitions
        
        protected abstract int GetObjectId(T obj);
        
        protected abstract Pose GetObjectPose(T obj);
        
        protected abstract void UpdateGameObject(T @object, GameObject gameObject);
        
        protected virtual void GameObjectDespawn(GameObject gameObject) { }

        protected abstract SlamPoint AsPoint(T obj);
        
        protected abstract T UpdateItem(T current, T @new);

        protected readonly SortedDictionary<int, T> Objects = new SortedDictionary<int, T>();
        
        protected readonly SortedDictionary<int, GameObject> GameObjects = new SortedDictionary<int, GameObject>();

        protected ObjectPool ObservationsPool;

        #endregion
    }
}
