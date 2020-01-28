using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamObservationsContainer : ICloudObjectsContainer<SlamObservation>
    {
        private readonly SortedDictionary<int, SlamObservation> m_nodes;
        private readonly SortedDictionary<int, GameObject> m_gameObjects;
        public ObjectPool ObservationsPool { get; private set; }

        public int Count => m_nodes.Count;

        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public SlamObservation this[SlamObservation obj]
        {
            get => this[obj.Point.id];
            set => this[obj.Point.id] = value;
        }

        /// <summary>
        /// Get clone of node or Set obj with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public SlamObservation this[int id]
        {
            get
            {
                //Debug.Assert(
                //    Exists(id),
                //    $"[SlamObservationsContainer.Get] Graph doesn't contain observation with id {id}");
                if (!Exists(id))
                    throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Get] Graph doesn't contain observation with id {id}");
                return m_nodes[id];
            }
            set
            {
                if (!TryGet(id, out _)) Add(value); else Update(value);
            }
        }

        /// <param name="prefab">Desired prefab of observation</param>
        /// <param name="lines">Lines cloud objects for connections drawing</param>
        public SlamObservationsContainer(GameObject prefab)
        {
            m_nodes = new SortedDictionary<int, SlamObservation>();
            m_gameObjects = new SortedDictionary<int, GameObject>();
            ObservationsPool = new ObjectPool(prefab);
        }

        /// <summary>
        /// Add ref of observation into the graph. Make sure that you don't use it anywhere!
        /// Pass the clone of observation if you are not sure.
        /// </summary>
        /// <param name="observation"></param>
        /// <returns>Id of observation</returns>
        public void Add(SlamObservation observation)
        {
            //Debug.Assert(
            //    !Exists(observation),
            //    $"[SlamObservationsContainer.Add] Graph already contains observation with id {observation.Point.id}");
            if (Exists(observation))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Add] Graph already contains observation with id {observation.Point.id}");
            m_nodes[observation.Point.id] = observation;
            m_gameObjects[observation.Point.id] = ObservationsPool.Spawn(observation.Point.position, observation.Orientation);
            Debug.Log(
                $"[SlamObservationsContainer.Add] Added observation with id {observation.Point.id}; count of covisible nodes {observation.CovisibleInfos.Count}");
        }

        /// <summary>
        /// Look at the summary of Add
        /// </summary>
        /// <param name="objects"></param>
        public void Add(IEnumerable<SlamObservation> objects)
        {
            foreach (var observation in objects)
            {
                Add(observation);
            }
        }

        /// <summary>
        /// Just change color field of SlamObservation point now
        /// </summary>
        /// <param name="obj"></param>
        public void ChangeColor(SlamObservation obj)
        {
            //Debug.Assert(
            //    Exists(obj),
            //    $"[SlamObservationsContainer.ChangeColor] Graph doesn't contain observation with id {obj.Point.id}");
            if (!Exists(obj))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.ChangeColor] Graph doesn't contain observation with id {obj.Point.id}");
            SlamObservation node = m_nodes[obj.Point.id];
            SlamPoint point = node.Point;
            point.color = obj.Point.color;
            node.Point = point;
        }

        /// <summary>
        /// Clear graph
        /// </summary>
        public void Clear()
        {
            ObservationsPool.DespawnAllActiveObjects();
            m_nodes.Clear();
        }

        /// <summary>
        /// Check node existence by SlamObservation object
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(SlamObservation obj) => Exists(obj.Point.id);

        /// <summary>
        /// Check existing of node by id
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(int objId) => m_nodes.ContainsKey(objId);

        /// <summary>
        /// Get clones of observations from graph
        /// </summary>
        /// <returns></returns>
        public SlamObservation[] GetAll() => m_nodes.Values.ToArray();

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            //Debug.Assert(
            //    Exists(id),
            //    $"[SlamObservationsContainer.Remove] Graph doesn't contain observation with id {id}");
            if (!Exists(id))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Remove] Graph doesn't contain observation with id {id}");

            Debug.Log(
                $"[SlamObservationsContainer.Remove] Removing observation with id {id}; count of covisible nodes {m_nodes[id].CovisibleInfos.Count}");
            ObservationsPool.Despawn(m_gameObjects[id]);
            m_gameObjects.Remove(id);
            m_nodes.Remove(id);
        }

        /// <summary>
        /// Remove by SlamObservation object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(SlamObservation obj) => Remove(obj.Point.id);

        /// <summary>
        /// Repaint connections
        /// </summary>
        public void Repaint() {}

        /// <summary>
        /// Try get observation from graph
        /// </summary>
        /// <param name="obj">Slam observation you want to find</param>
        /// <param name="current">Clone of observation in graph or null if doesn't exist</param>
        /// <returns>Returns false if observation with given id from obj exists, otherwise true.</returns>
        public bool TryGet(SlamObservation obj, out SlamObservation current) => TryGet(obj.Point.id, out current);

        public bool TryGet(int idx, out SlamObservation current) => m_nodes.TryGetValue(idx, out current);

        /// <summary>
        /// Copy data from obj.
        /// </summary>
        /// <param name="obj"></param>
        public void Update(SlamObservation obj)
        {
            //Debug.Assert(
            //    Exists(obj.Point.id),
            //    $"[SlamObservationsContainer.Update] Graph doesn't contain observation with id {obj.Point.id}");
            if (!Exists(obj.Point.id))
                throw new InvalidSlamContainerOperationException($"[SlamObservationsContainer.Update] Graph doesn't contain observation with id {obj.Point.id}");
            SlamObservation node = m_nodes[obj.Point.id];
            node.Statistics = obj.Statistics;
            node.Point = obj.Point;
            node.Orientation = obj.Orientation;
            var objectTransform = m_gameObjects[node.Point.id].transform;
            objectTransform.position = node.Point.position;
            objectTransform.rotation = node.Orientation;
            Debug.Log(
                $"[SlamObservationsContainer.Update] Updated observation with id {node.Point.id}; count of covisible nodes {node.CovisibleInfos.Count}");
        }
        
        public IEnumerator<SlamObservation> GetEnumerator() => m_nodes.Values.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => m_nodes.Values.GetEnumerator();

        public bool TryGetAsPoint(SlamObservation obj, out SlamPoint point) => TryGetAsPoint(obj.Point.id, out point);

        public bool TryGetAsPoint(int idx, out SlamPoint point)
        {
            point = new SlamPoint();
            if (TryGet(idx, out var result))
            {
                point = result;
                return true;
            }
            return false;
        }

        public void Update(IEnumerable<SlamObservation> objs)
        {
            foreach (var obj in objs)
                Update(obj);
        }

        public void Remove(IEnumerable<SlamObservation> objs)
        {
            foreach (var obj in objs)
                Remove(obj);
        }
    }
}
