using Elektronik.Common.Clouds;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamObservationsContainer : ISlamContainer<SlamObservation>
    {
        private List<SlamObservation> m_nodes;
        private Dictionary<int, GameObject> m_gameObjects;
        private Dictionary<int, Guid> m_connections;

        private GameObject m_observationPrefab;
        private FastLinesCloud m_linesCloud;

        /// <summary>
        /// Make sure that the prefab was registered in MF_Autopool
        /// </summary>
        /// <param name="prefab">Desired prefab of observation</param>
        /// <param name="linesCloud">Lines cloud objects for connections drawing</param>
        public SlamObservationsContainer(GameObject prefab, FastLinesCloud linesCloud)
        {
            m_nodes = new List<SlamObservation>();
            m_gameObjects = new Dictionary<int, GameObject>();
            m_observationPrefab = prefab;
            m_linesCloud = linesCloud;
            m_connections = new Dictionary<int, Guid>();
        }

        private void DisconnectFromAll(SlamObservation observation)
        {

        }

        private void UpdateConnectionsOf(SlamObservation observation)
        {

        }

        private void UpdateGameobjectOf(SlamObservation observation)
        {
            var objectTransform = m_gameObjects[observation.Point.id].transform;
            objectTransform.position = observation.Point.position;
            objectTransform.rotation = observation.Orientation;
        }


        /// <summary>
        /// Add ref of observation into the graph. Make sure that you don't use it anywhere!
        /// Pass the clone of observation if you are not sure.
        /// </summary>
        /// <param name="observation"></param>
        /// <returns>Id of observation</returns>
        public int Add(SlamObservation observation)
        {
            if (observation == null)
                Debug.LogWarning("[SlamObservationsContainer.Add] Null observation");
            Debug.AssertFormat(
                !Exists(observation),
                "[SlamObservationsContainer.Add] Graph already contains observation with id {0}", observation.Point.id);
            m_nodes.Add(observation);
            m_gameObjects[observation.Point.id] = MF_AutoPool.Spawn(m_observationPrefab);
            UpdateConnectionsOf(observation);
            UpdateGameobjectOf(observation);
            return observation.Point.id;
        }

        /// <summary>
        /// Look at the summary of Add
        /// </summary>
        /// <param name="objects"></param>
        public void AddRange(SlamObservation[] objects)
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
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.ChangeColor] Null observation");
            Debug.AssertFormat(
                Exists(obj),
                "[SlamObservationsContainer.ChangeColor] Graph doesn't contain observation with id {0}", obj.Point.id);
            SlamObservation node = m_nodes.First(n => obj.Point.id == n.Point.id);
            SlamPoint point = node.Point;
            point.color = obj.Point.color;
            node.Point = point;
        }

        /// <summary>
        /// Clear graph
        /// </summary>
        public void Clear()
        {
            m_nodes.Clear();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check existing of node by SlamObservation object
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Exists] Null observation");
            return Exists(obj.Point.id);
        }

        /// <summary>
        /// Check existing of node by id
        /// </summary>
        /// <param name="objId"></param>
        /// <returns>true if exists, otherwise false</returns>
        public bool Exists(int objId)
        {
            return m_nodes.Any(obs => obs.Point.id == objId);
        }

        /// <summary>
        /// Get clone of node with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public SlamObservation Get(int id)
        {
            Debug.AssertFormat(
                Exists(id),
                "[SlamObservationsContainer.Get] Graph doesn't contain observation with id {0}", id);
            return new SlamObservation(m_nodes.Find(obs => obs.Point.id == id), false);
        }

        /// <summary>
        /// Get clone of node with same id as argument id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Clone of SlamObservation from graph</returns>
        public SlamObservation Get(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Get] Null observation");
            return Get(obj.Point.id);
        }

        /// <summary>
        /// Get clones of observations from graph
        /// </summary>
        /// <returns></returns>
        public SlamObservation[] GetAll()
        {
            return m_nodes.Select(node => new SlamObservation(node, false)).ToArray();
        }

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            Debug.AssertFormat(
                Exists(id),
                "[SlamObservationsContainer.Remove] Graph doesn't contain observation with id {0}", id);

            MF_AutoPool.Despawn(m_gameObjects[id]);
            m_gameObjects.Remove(id);
            m_nodes.Remove(Get(id));
        }

        /// <summary>
        /// Remove by SlamObservation object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Remove] Null observation");
            Remove(obj.Point.id);
        }

        /// <summary>
        /// Repaint connections
        /// </summary>
        public void Repaint()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add or hard set obj ref into graph
        /// </summary>
        /// <param name="obj"></param>
        public void Set(SlamObservation obj)
        {
            SlamObservation buttPlug;
            if (!TryGet(obj, out buttPlug))
            {
                Add(obj);
            }
            else
            {
                int index = m_nodes.FindIndex(o => o.Point.id == obj.Point.id);
                m_nodes[index] = obj;
                UpdateGameobjectOf(obj);
                UpdateConnectionsOf(obj);
            }
        }

        /// <summary>
        /// Try get observation from graph
        /// </summary>
        /// <param name="obj">Slam observation you want to find</param>
        /// <param name="current">Clone of observation in graph or null if doesn't exist</param>
        /// <returns>Returns false if observation with given id from obj exists, otherwise true.</returns>
        public bool TryGet(SlamObservation obj, out SlamObservation current)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.TryGet] Null observation");
            current = new SlamObservation(m_nodes.FirstOrDefault(o => o.Point.id == obj.Point.id), false);
            return current == null;
        }

        /// <summary>
        /// Unlike Set this function does not replace reference to Covisible observations, 
        /// it updates covisible observations list of graph node by adding missing observations from obj.
        /// In the rest behavior is same to Set.
        /// </summary>
        /// <param name="obj"></param>
        public void Update(SlamObservation obj)
        {
            if (obj == null)
                Debug.LogWarning("[SlamObservationsContainer.Update] Null observation");
            Debug.AssertFormat(
                Exists(obj.Point.id),
                "[SlamObservationsContainer.Update] Graph doesn't contain observation with id {0}", obj.Point.id);
            SlamObservation node = m_nodes.First(obs => obs.Point.id == obj.Point.id);
            node.Statistics = obj.Statistics;
            node.Point = obj.Point;
            node.Orientation = obj.Orientation;
            UpdateGameobjectOf(node);

            foreach (var covisible in obj.CovisibleObservationsInfo)
            {
                if (!node.CovisibleObservationsInfo.Contains(covisible))
                    node.CovisibleObservationsInfo.Add(covisible);
            }

            UpdateConnectionsOf(node);
        }
    }
}
