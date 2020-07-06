using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public abstract class GameObjectsContainer<T> : ICloudObjectsContainer<T>
    {
        protected abstract int GetObjectId(T obj);
        protected abstract Pose GetObjectPose(T obj);
        protected abstract void UpdateGameObject(T @object, GameObject gameObject);
        protected virtual void GameObjectDespawn(GameObject gameObject) { }

        protected abstract SlamPoint AsPoint(T obj);
        protected abstract T Update(T current, T @new);

        protected readonly MainThreadInvoker m_invoker;
        protected readonly SortedDictionary<int, T> m_objects;
        protected readonly SortedDictionary<int, GameObject> m_gameObjects;
        public ObjectPool ObservationsPool { get; private set; }

        public int Count => m_objects.Count;

        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
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
            get
            {
                if (!Exists(id))
                    throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Get] Graph doesn't contain observation with id {id}");
                return m_objects[id];
            }
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        /// <param name="prefab">Desired prefab of observation</param>
        /// <param name="lines">Lines cloud objects for connections drawing</param>
        public GameObjectsContainer(GameObject prefab, MainThreadInvoker invoker)
        {
            m_objects = new SortedDictionary<int, T>();
            m_gameObjects = new SortedDictionary<int, GameObject>();
            m_invoker = invoker;
            // m_mainTreadActions = new Queue<(T, ActionType)>();
            ObservationsPool = new ObjectPool(prefab);
            ObservationsPool.OnObjectDeSpawn += (o, s) => GameObjectDespawn(o);
        }

        /// <summary>
        /// Add ref of observation into the graph. Make sure that you don't use it anywhere!
        /// Pass the clone of observation if you are not sure.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Id of observation</returns>
        public void Add(T obj)
        {
            int id = GetObjectId(obj);
            if (Exists(obj))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Add] Graph already contains {typeof(T).Name} with id {id}");
            m_objects[id] = obj;

            Pose pose = GetObjectPose(obj);
            m_invoker.Enqueue(() => m_gameObjects[GetObjectId(obj)] = ObservationsPool.Spawn(pose.position, pose.rotation));
            Debug.Log($"[SlamObservationsContainer.Add] Added {typeof(T).Name} with id {id}");
        }

        /// <summary>
        /// Look at the summary of Add
        /// </summary>
        /// <param name="objects"></param>
        public void Add(IEnumerable<T> objects)
        {
            foreach (var observation in objects)
            {
                Add(observation);
            }
        }

        /// <summary>
        /// Clear graph
        /// </summary>
        public void Clear()
        {
            m_gameObjects.Clear();
            m_objects.Clear();
            m_invoker.Enqueue(() => ObservationsPool.DespawnAllActiveObjects());
        }

        /// <summary>
        /// Check node existence by SlamObservation object
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(T obj) => Exists(GetObjectId(obj));

        /// <summary>
        /// Check existing of node by id
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(int objId) => m_objects.ContainsKey(objId);

        /// <summary>
        /// Get clones of observations from graph
        /// </summary>
        /// <returns></returns>
        public T[] GetAll() => m_objects.Values.ToArray();

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            if (!Exists(id))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Remove] Graph doesn't contain observation with id {id}");
            var object2Remove = m_gameObjects[id];
            m_invoker.Enqueue(() => ObservationsPool.Despawn(object2Remove));
            m_gameObjects.Remove(id);
            m_objects.Remove(id);
            Debug.Log($"[SlamObservationsContainer.Remove] Removing observation with id {id}");
        }

        /// <summary>
        /// Remove by SlamObservation object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj) => Remove(GetObjectId(obj));

        public bool TryGet(T obj, out GameObject gameObject)
        {
            gameObject = null;
            if (Exists(GetObjectId(obj)))
            {
                gameObject = m_gameObjects[GetObjectId(obj)];
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

        public bool TryGet(int idx, out T current) => m_objects.TryGetValue(idx, out current);

        /// <summary>
        /// Copy data from obj.
        /// </summary>
        /// <param name="obj"></param>
        public void Update(T obj)
        {
            if (!Exists(GetObjectId(obj)))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Update] Graph doesn't contain {typeof(T).Name} with id {GetObjectId(obj)}");
            m_objects[GetObjectId(obj)] = Update(m_objects[GetObjectId(obj)], obj);
            var object2Update = m_gameObjects[GetObjectId(obj)];
            m_invoker.Enqueue(() => UpdateGameObject(obj, object2Update));
            Debug.Log($"[SlamObservationsContainer.Update] Updated {typeof(T).Name} with id {GetObjectId(obj)}");
        }

        public IEnumerator<T> GetEnumerator() => m_objects.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_objects.Values.GetEnumerator();

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

        public void Update(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
                Update(obj);
        }

        public void Remove(IEnumerable<T> objs)
        {
            foreach (var obj in objs)
                Remove(obj);
        }
    }
}
