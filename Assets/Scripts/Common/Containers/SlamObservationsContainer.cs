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
        private List<Connection> m_connections;

        private GameObject m_observationPrefab;
        private ISlamContainer<SlamLine> m_lines;

        private struct Connection
        {
            public readonly SlamObservation first;
            public readonly SlamObservation second;
            public readonly int id;

            public Connection(SlamObservation first, SlamObservation second, int id)
            {
                this.first = first;
                this.second = second;
                this.id = id;
            }
        }

        /// <summary>
        /// Make sure that the prefab was registered in MF_Autopool
        /// </summary>
        /// <param name="prefab">Desired prefab of observation</param>
        /// <param name="lines">Lines cloud objects for connections drawing</param>
        public SlamObservationsContainer(GameObject prefab, ISlamContainer<SlamLine> lines)
        {
            m_nodes = new List<SlamObservation>();
            m_gameObjects = new Dictionary<int, GameObject>();
            m_observationPrefab = prefab;
            m_lines = lines;
            m_connections = new List<Connection>();
        }

        private void DisconnectFromAll(SlamObservation observation)
        {
            foreach (var connection in m_connections)
            {
                if (connection.first == observation || connection.second == observation)
                    m_lines.Remove(connection.id);
            }
            m_connections.RemoveAll(c => c.first == observation || c.second == observation);
        }

        private void UpdateConnectionsOf(SlamObservation observation)
        {
            foreach (var covisible in observation.CovisibleInfos)
            {
                // 1. Найти существующих в графе соседей
                SlamObservation neighbour = m_nodes.FirstOrDefault(node => node.Point.id == covisible.id);
                if (neighbour == null)
                    continue;
                // 2. Проверить наличие соединения между ними
                int connectionIdx = m_connections.FindIndex(c =>
                    c.first == observation && c.second == neighbour ||
                    c.second == observation && c.first == neighbour);
                if (connectionIdx == -1)
                {
                    // 3. Где соединение отсутствует, добавить соединение
                    var line = new SlamLine(
                        observation.Point.position, neighbour.Point.position,
                        observation.Point.id, neighbour.Point.id,
                        Color.gray, Color.gray);
                    m_connections.Add(new Connection(observation, neighbour, m_lines.Add(line)));
                }
                else
                {
                    // 4. Обновить вершины соединений
                    Connection connection = m_connections[connectionIdx];
                    SlamLine line = m_lines.Get(connection.id);
                    if (connection.first == observation)
                    {
                        line.vert1 = observation.Point.position;
                        line.vert2 = neighbour.Point.position;
                    }
                    else
                    {
                        line.vert2 = observation.Point.position;
                        line.vert1 = neighbour.Point.position;
                    }
                    m_lines.Update(line);
                }
            }
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
            m_gameObjects[observation.Point.id] = MF_AutoPool.Spawn(m_observationPrefab, observation.Point.position, observation.Orientation);
            UpdateConnectionsOf(observation);
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
            MF_AutoPool.DespawnPool(m_observationPrefab);
            m_lines.Clear();
            m_connections.Clear();
            m_nodes.Clear();
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

            var node = m_nodes.First(obs => obs.Point.id == id);
            DisconnectFromAll(node);
            MF_AutoPool.Despawn(m_gameObjects[id]);
            m_gameObjects.Remove(id);
            m_nodes.Remove(node);
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
            m_lines.Repaint();
        }

        /// <summary>
        /// Add or Update node by obj
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
                Update(obj);
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
        /// Copy data from obj.
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

            node.CovisibleInfos.Clear();
            foreach (var covisible in obj.CovisibleInfos)
            {
                node.CovisibleInfos.Add(new SlamObservation.CovisibleInfo()
                {
                    id = covisible.id,
                    sharedPointsCount = covisible.sharedPointsCount
                });
            }

            UpdateConnectionsOf(node);
        }
    }
}
