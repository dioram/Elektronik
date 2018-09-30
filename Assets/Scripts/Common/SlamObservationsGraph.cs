using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    [RequireComponent(typeof(FastLinesCloud))]
    public class SlamObservationsGraph : MonoBehaviour
    {
        public GameObject observationPrefab;

        SlamLinesContainer m_linesContainer;

        SortedDictionary<int, SlamObservationNode> m_slamObservationNodes;

        private void Awake()
        {
            m_slamObservationNodes = new SortedDictionary<int, SlamObservationNode>();
            m_linesContainer = new SlamLinesContainer(GetComponent<FastLinesCloud>());
        }

        /// <summary>
        /// Обновить соединения для узла графа
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateConnections(SlamObservation observation)
        {
            if (observation.id == -1)
                return;
            Debug.AssertFormat(ObservationExists(observation.id), "[Graph update connections] observation {0} doesn't exists", observation.id);
            
            SlamObservationNode obsNode = m_slamObservationNodes[observation.id];
            obsNode.SlamObservation.position = observation.position;

            // 1. Найти существующих в графе соседей
            SlamObservationNode[] existsNeighbors = observation.covisibleObservationsIds
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
                        color = Color.gray,
                        pointId1 = obsNode.SlamObservation.id,
                        pointId2 = neighbor.SlamObservation.id,
                        isRemoved = false,
                        vert1 = obsNode.SlamObservation.position,
                        vert2 = neighbor.SlamObservation.position,
                    };
                    int lineId = m_linesContainer.Add(newLineCinema);
                    neighbor.NodeLineIDPair.Add(obsNode, lineId);
                    obsNode.NodeLineIDPair.Add(neighbor, lineId);
                }
                // 4. Обновить вершины соединений
                int existsLineId = obsNode.NodeLineIDPair[neighbor];
                SlamLine currentConnection = m_linesContainer.GetLine(existsLineId);
                if (obsNode.SlamObservation.id == currentConnection.pointId1)
                {
                    currentConnection.vert1 = obsNode.SlamObservation.position;
                    currentConnection.vert2 = neighbor.SlamObservation.position;
                }
                else
                {
                    currentConnection.vert1 = neighbor.SlamObservation.position;
                    currentConnection.vert2 = obsNode.SlamObservation.position;
                }
                m_linesContainer.Update(currentConnection);
            }
        }

        /// <summary>
        /// Добавить Observation
        /// </summary>
        /// <param name="observation"></param>
        public void AddNewObservation(SlamObservation observation)
        {
            if (observation.covisibleObservationsIds == null || observation.covisibleObservationsOfCommonPointsCount == null)
            {
                Debug.LogWarningFormat("Wrong observation id {0}", observation.id);
                return;
            }
            var newNode = new SlamObservationNode(MF_AutoPool.Spawn(observationPrefab, observation.position, observation.orientation)); // создаём новый узел
            
            newNode.SlamObservation = observation;
            m_slamObservationNodes.Add(observation.id, newNode);

            // обновляем связи
            UpdateConnections(observation);

            /*int countOfCovisibleObservations = observation.covisibleObservationsIds.Length; // определяем, сколько совидимых узлов

            for (int covisibleIndx = 0; covisibleIndx < countOfCovisibleObservations; ++covisibleIndx)
            {
                // находим совидимый новому узлу узел
                int covisibleId = observation.covisibleObservationsIds[covisibleIndx];
                SlamObservationNode covisibleObservationNode = m_slamObservationNodes[covisibleId];

                SlamLine newLineCinema = new SlamLine()
                {
                    pointId1 = covisibleObservationNode.SlamObservation.id,
                    pointId2 = newNode.SlamObservation.id,
                    vert1 = covisibleObservationNode.Position,
                    vert2 = newNode.Position,
                    color = Color.gray,
                };
                // рисуем грань
                int lineId = m_linesContainer.Add(newLineCinema);

                //m_fastLinesCloud.GetIdxFor2VertIds(covisibleObservationNode.SlamObservation.id, newNode.SlamObservation.id);

                // совидимому узлу добавляем новый узел и индекс грани соединения с ним
                covisibleObservationNode.NodeLineIDPair.Add(newNode, lineId);

                // новому узлу добавляем совидимый узел и индекс грани соединения с ним
                newNode.NodeLineIDPair.Add(covisibleObservationNode, lineId);

                // рисуем грань
                //m_fastLinesCloud.SetLine(lineId, newNode.Position, covisibleObservationNode.Position, Color.gray);
            }*/

        }

        /// <summary>
        /// Удалить Observation
        /// </summary>
        /// <param name="observationId"></param>
        public void RemoveObservation(int observationId)
        {
            // находим узел, который необходимо удалить
            SlamObservationNode observationToRemove = m_slamObservationNodes[observationId];

            // находим узлы из которых нужно выпилить текущий узел
            SlamObservationNode[] covisibleNodes = observationToRemove.NodeLineIDPair.Select(kv => kv.Key).ToArray();
            
            // выпиливаем узел из совидимых узлов и очищаем облако
            foreach (var covisibleNode in covisibleNodes)
            {
                int lineId = covisibleNode.NodeLineIDPair[observationToRemove]; // ID линии, которую нужно убрать
                m_linesContainer.Remove(lineId);
                //m_fastLinesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0)); // удаляем грань
                covisibleNode.NodeLineIDPair.Remove(observationToRemove);
            }

            MF_AutoPool.Despawn(observationToRemove.ObservationObject); // выпиливаем со сцены
            m_slamObservationNodes.Remove(observationId); // выпиливаем из графа
        }

        /// <summary>
        /// Переместить Observation
        /// </summary>
        /// <param name="observation">Observation с абсолютными координатами</param>
        public void ReplaceObservation(SlamObservation observation)
        {
            Debug.AssertFormat(m_slamObservationNodes.ContainsKey(observation.id), "Observation with Id {0} doesn't exist", observation.id);
            
            // находим узел, который необходимо переместить
            SlamObservationNode nodeToReplace = m_slamObservationNodes[observation.id];

            /*foreach (var covisibleNode in nodeToReplace.NodeLineIDPair)
            {
                // получаем грань, которую необходимо изменить

                SlamLine line = m_linesContainer.GetLine(covisibleNode.Value);
                //m_fastLinesCloud.GetLine(covisibleNode.Value, out position1, out position2, out color);

                // определяем торцы изменённой грани
                // если позиция совпадает, то это торец, который нужно поменять
                line.vert1 = line.vert1 == nodeToReplace.Position ? observation.position : line.vert1;
                line.vert2 = line.vert2 == nodeToReplace.Position ? observation.position : line.vert2;

                // записываем изменение в облако
                m_linesContainer.Update(line);

                //m_fastLinesCloud.SetLinePosition(covisibleNode.Value, position1, position2);
            }*/

            // Перемещаем в сцене
            nodeToReplace.ObservationObject.transform.position = observation.position;
            nodeToReplace.ObservationObject.transform.rotation = observation.orientation;

            nodeToReplace.SlamObservation.position = observation.position;
            nodeToReplace.SlamObservation.orientation = observation.orientation;

            // Обновляем связи
            UpdateConnections(nodeToReplace.SlamObservation);
        }

        /// <summary>
        /// Полностью обновить информацию об Observation
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateObservation(SlamObservation observation)
        {
            Debug.AssertFormat(m_slamObservationNodes.ContainsKey(observation.id), "Observation with Id {0} doesn't exist", observation.id);

            RemoveObservation(observation.id);
            AddNewObservation(observation);
        }

        /// <summary>
        /// Обновить информацию об Observation, либо добавить его, если он не существует в графе
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateOrAddObservation(SlamObservation observation)
        {
            if (ObservationExists(observation.id))
            {
                UpdateObservation(observation);
            }
            else
            {
                AddNewObservation(observation);
            }
        }

        public SlamObservation GetObservation(int observationId)
        {
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

        public SlamObservation[] GetAllSlamObservations()
        {
            return m_slamObservationNodes.Select(obsNode => obsNode.Value.SlamObservation).ToArray();
        }
    }
}
