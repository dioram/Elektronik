using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Elektronik.Common.Containers;
using Elektronik.Common.Clouds;

namespace Elektronik.Common
{
    [RequireComponent(typeof(FastLinesCloud), typeof(FastTrianglesCloud))]
    public class SlamObservationsGraph : MonoBehaviour
    {
        ISlamLinesContainer<SlamLine> m_linesContainer;
        ISlamContainer<SlamObservation> m_observationsContainer;

        private void Awake()
        {
            m_linesContainer = new SlamLinesContainer(GetComponent<FastLinesCloud>());
            m_observationsContainer = new SlamTetrahedronObservationsContainer(GetComponent<FastTrianglesCloud>());
        }

        /// <summary>
        /// Добавить Observation
        /// </summary>
        /// <param name="observation"></param>
        public void Add(SlamObservation observation)
        {
            if (observation.id == -1) Debug.LogWarning("[Graph] invalid observation id (-1)");
            if (observation.covisibleObservationsIds == null || observation.covisibleObservationsOfCommonPointsCount == null)
            {
                Debug.LogWarningFormat("Wrong observation id {0}", observation.id);
                return;
            }
            m_observationsContainer.Add(new SlamObservation(observation)); // создаём новый узел
            Repaint();
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
            // находим узел, который необходимо переместить
            SlamObservation nodeToReplace = m_observationsContainer.Get(observation.id);
            // Перемещаем в сцене
            nodeToReplace.position = observation.position;
            nodeToReplace.orientation = observation.orientation;
            m_observationsContainer.Set(nodeToReplace);
            // Обновляем связи
            UpdateConnections(nodeToReplace);
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
            
            SlamObservation obsNode = m_observationsContainer.Get(observation.id);
            obsNode.covisibleObservationsIds = observation.covisibleObservationsIds;
            obsNode.covisibleObservationsOfCommonPointsCount = observation.covisibleObservationsOfCommonPointsCount;

            // 1. Найти существующих в графе соседей
            SlamObservation[] existsNeighbors = 
                observation.covisibleObservationsIds
                .Where(ObservationExists)
                .Select(m_observationsContainer.Get)
                .ToArray();

            foreach (var neighbor in existsNeighbors)
            {
                // 2. Проверить наличие соединения между ними
                SlamLine line;
                if (!m_linesContainer.TryGet(obsNode.id, neighbor.id, out line))
                {
                    // 3. Где соединение отсутствует добавить соединение
                    line = new SlamLine()
                    {
                        color1 = Color.gray,
                        pointId1 = obsNode.id,
                        pointId2 = neighbor.id,
                        isRemoved = false,
                        vert1 = obsNode.position,
                        vert2 = neighbor.position,
                    };
                    m_linesContainer.Add(line);
                }
                else // 4. Обновить вершины соединений
                {
                    if (obsNode.id == line.pointId1)
                    {
                        line.vert1 = obsNode.position;
                        line.vert2 = neighbor.position;
                    }
                    else if (obsNode.id == line.pointId2)
                    {
                        line.vert1 = neighbor.position;
                        line.vert2 = obsNode.position;
                    }
                    m_linesContainer.Update(line);
                }
            }
        }

        /// <summary>
        /// Удалить Observation
        /// </summary>
        /// <param name="observationId"></param>
        public void Remove(int observationId)
        {
            // находим узел, который необходимо удалить
            SlamObservation observationToRemove = m_observationsContainer.Get(observationId);
            m_observationsContainer.Remove(observationId); // выпиливаем со сцены

            // находим узлы из которых нужно выпилить текущий узел
            SlamObservation[] covisibleNodes = observationToRemove
                .covisibleObservationsIds
                .Where(id => m_observationsContainer.Exists(id))
                .Select(id => m_observationsContainer.Get(id))
                .ToArray();
            
            // выпиливаем узел из совидимых узлов и очищаем облако
            foreach (var covisibleNode in covisibleNodes)
            {
                m_linesContainer.Remove(observationToRemove.id, covisibleNode.id);
            }
        }

        /// <summary>
        /// Полностью обновить информацию об Observation
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateNode(SlamObservation observation)
        {
            Remove(observation.id);
            Add(observation);
        }

        /// <summary>
        /// Обновить информацию об Observation, либо добавить его, если он не существует в графе
        /// </summary>
        /// <param name="observation"></param>
        public void UpdateOrAdd(SlamObservation observation)
        {
            if (ObservationExists(observation.id))
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
            return m_observationsContainer.Get(observationId);
        }

        public bool ObservationExists(int observationId)
        {
            return m_observationsContainer.Exists(observationId);
        }

        public void Repaint()
        {
            m_observationsContainer.Repaint();
            m_linesContainer.Repaint();
        }

        public void Clear()
        {
            m_observationsContainer.Clear();
            m_linesContainer.Clear();
        }

        public SlamObservation[] GetAll()
        {
            return m_observationsContainer.GetAll();
        }
    }
}
