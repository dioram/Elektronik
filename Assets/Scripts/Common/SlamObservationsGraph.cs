using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Elektronik.Common.Containers;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data;

namespace Elektronik.Common
{
    [RequireComponent(typeof(FastLinesCloud))]
    public class SlamObservationsGraph : MonoBehaviour
    {
        public GameObject observationPrefab;

        ISlamContainer<SlamLine> m_linesContainer;

        SortedDictionary<int, SlamObservationNode> m_slamObservationNodes;

        private void Awake()
        {
            m_slamObservationNodes = new SortedDictionary<int, SlamObservationNode>();
            m_linesContainer = new SlamLinesContainer(GetComponent<FastLinesCloud>());
        }

        /// <summary>
        /// Добавить Observation
        /// </summary>
        /// <param name="observation"></param>
        public void Add(SlamObservation observation)
        {
            if (observation.CovisibleInfos == null)
            {
                Debug.LogWarningFormat("Wrong observation id {0}", observation.Point.id);
                return;
            }
            var newNode = new SlamObservationNode(MF_AutoPool.Spawn(observationPrefab, observation.Point.position, observation.Orientation)) // создаём новый узел
            {
                SlamObservation = new SlamObservation(observation),
            };
            m_slamObservationNodes.Add(observation.Point.id, newNode);

            // обновляем связи
            UpdateConnections(observation);
        }

        /// <summary>
        /// Переместить Observation
        /// </summary>
        /// <param name="observation">Observation с абсолютными координатами</param>
        public void Replace(SlamObservation observation)
        {
            //Debug.Log("[SlamObservationsGraph.Replace] Replacing");
            Debug.AssertFormat(
                m_slamObservationNodes.ContainsKey(observation.Point.id),
                "Observation with Id {0} doesn't exist",
                observation.Point.id);

            // находим узел, который необходимо переместить
            SlamObservationNode nodeToReplace = m_slamObservationNodes[observation.Point.id];

            // Перемещаем в сцене
            nodeToReplace.ObservationObject.transform.position = observation.Point.position;
            nodeToReplace.ObservationObject.transform.rotation = observation.Orientation;

            SlamPoint nodeToReplacePoint = nodeToReplace.SlamObservation;
            nodeToReplacePoint.position = observation.Point.position;
            nodeToReplace.SlamObservation.Point = nodeToReplacePoint;
            nodeToReplace.SlamObservation.Orientation = observation.Orientation;

            // Обновляем связи
            UpdateConnections(nodeToReplace.SlamObservation);
        }

        /// <summary>
        /// Обновить соединения для узла графа
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateConnections(SlamObservation observation)
        {
            if (observation.Point.id == -1)
                return;
            Debug.AssertFormat(
                ObservationExists(observation.Point.id),
                "[Graph update connections] observation {0} doesn't exists",
                observation.Point.id);

            SlamObservationNode obsNode = m_slamObservationNodes[observation.Point.id];
            obsNode.SlamObservation = new SlamObservation(observation, true);

            // 1. Найти существующих в графе соседей
            SlamObservationNode[] existsNeighbors = observation.CovisibleInfos
                .Select(obs => obs.id)
                .Where(ObservationExists)
                .Select(obsId => m_slamObservationNodes[obsId])
                .ToArray();

            foreach (var neighbor in existsNeighbors)
            {
                // 2. Проверить наличие соединения между ними
                if (!neighbor.NodeLineIDPair.ContainsKey(obsNode))
                {
                    // 3. Где соединение отсутствует добавить соединение
                    SlamLine newLineCinema = new SlamLine()
                    {
                        color1 = Color.gray,
                        pointId1 = obsNode.SlamObservation.Point.id,
                        pointId2 = neighbor.SlamObservation.Point.id,
                        isRemoved = false,
                        vert1 = obsNode.SlamObservation.Point.position,
                        vert2 = neighbor.SlamObservation.Point.position,
                    };
                    int lineId = m_linesContainer.Add(newLineCinema);
                    neighbor.NodeLineIDPair.Add(obsNode, lineId);
                    obsNode.NodeLineIDPair.Add(neighbor, lineId);
                }
            }

            // 4. Обновить вершины соединений
            foreach (var connection in obsNode.NodeLineIDPair)
            {
                int existsLineId = connection.Value;
                SlamLine currentConnection = m_linesContainer.Get(existsLineId);

                Debug.Assert(
                    obsNode.SlamObservation.Point.id == currentConnection.pointId1 || 
                    obsNode.SlamObservation.Point.id == currentConnection.pointId2,
                    "[SlamObservationGraph.UpdateConnections] at least one of vertex point id must be equal to observation id");

                if (obsNode.SlamObservation.Point.id == currentConnection.pointId1)
                {
                    currentConnection.vert1 = obsNode.SlamObservation.Point.position;
                    currentConnection.vert2 = connection.Key.SlamObservation.Point.position;
                }
                else if (obsNode.SlamObservation.Point.id == currentConnection.pointId2)
                {
                    currentConnection.vert1 = connection.Key.SlamObservation.Point.position;
                    currentConnection.vert2 = obsNode.SlamObservation.Point.position;
                }
                m_linesContainer.Update(currentConnection);
            }
        }

        /// <summary>
        /// Удалить Observation
        /// </summary>
        /// <param name="observationId"></param>
        public void Remove(int observationId)
        {
            // находим узел, который необходимо удалить
            SlamObservationNode observationToRemove = m_slamObservationNodes[observationId];
            MF_AutoPool.Despawn(observationToRemove.ObservationObject); // выпиливаем со сцены

            // находим узлы из которых нужно выпилить текущий узел
            SlamObservationNode[] covisibleNodes = observationToRemove.NodeLineIDPair.Select(kv => kv.Key).ToArray();

            // выпиливаем узел из совидимых узлов и очищаем облако
            foreach (var covisibleNode in covisibleNodes)
            {
                int lineId = covisibleNode.NodeLineIDPair[observationToRemove]; // ID линии, которую нужно убрать
                covisibleNode.NodeLineIDPair.Remove(observationToRemove);
                m_linesContainer.Remove(lineId);
            }
            m_slamObservationNodes.Remove(observationId); // выпиливаем из графа
        }

        /// <summary>
        /// Полностью обновить информацию об Observation
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateNode(SlamObservation observation)
        {
            Debug.AssertFormat(
                m_slamObservationNodes.ContainsKey(observation.Point.id),
                "Observation with Id {0} doesn't exist",
                observation.Point.id);

            Remove(observation.Point.id);
            Add(observation);
        }

        /// <summary>
        /// Обновить информацию об Observation, либо добавить его, если он не существует в графе
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateOrAdd(SlamObservation observation)
        {
            if (ObservationExists(observation.Point.id))
            {
                UpdateNode(observation);
            }
            else
            {
                Add(observation);
            }
        }

        public SlamObservation Get(int observationId)
        {
            Debug.AssertFormat(ObservationExists(observationId), "[SlamObservationsGraph.Get] graph doesn't contain observation {0}", observationId);
            return m_slamObservationNodes[observationId].SlamObservation;
        }

        public bool ObservationExists(int observationId)
        {
            return m_slamObservationNodes.ContainsKey(observationId);
        }

        public void Repaint()
        {
            m_linesContainer.Repaint();
        }

        public void Clear()
        {
            m_slamObservationNodes.Clear();
            m_linesContainer.Clear();
            MF_AutoPool.DespawnPool(observationPrefab);
        }

        public void SetActive(bool value)
        {
            observationPrefab.SetActive(value);
            MF_AutoPool.ForEach(observationPrefab, obj => obj.SetActive(value));
        }

        public SlamObservation[] GetAll()
        {
            return m_slamObservationNodes.Select(obsNode => obsNode.Value.SlamObservation).ToArray();
        }
    }
}
