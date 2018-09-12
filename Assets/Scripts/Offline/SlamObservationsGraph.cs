using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    [RequireComponent(typeof(FastLinesCloud))]
    public class SlamObservationsGraph : MonoBehaviour
    {
        public GameObject observationPrefab;

        FastLinesCloud m_fastLinesCloud;
        SortedDictionary<int, SlamObservationNode> m_slamObservationNodes;

        private void Awake()
        {
            m_slamObservationNodes = new SortedDictionary<int, SlamObservationNode>();
        }

        private void Start()
        {
            m_fastLinesCloud = GetComponent<FastLinesCloud>();
        }

        public void AddNewObservation(SlamObservation observation)
        {
            var newNode = new SlamObservationNode(MF_AutoPool.Spawn(observationPrefab, observation.position, observation.orientation)); // создаём новый узел
            newNode.SlamObservation = observation;
            newNode.Position = observation.position; // присваиваем ему положение
            int countOfCovisibleObservations = observation.covisibleObservationsIds.Length; // определяем, сколько совидимых узлов
            int[] linesIds = m_fastLinesCloud.GetNewLineIdxs(countOfCovisibleObservations); // для каждого совидимого получаем индекс новой границы

            for (int covisibleIndx = 0; covisibleIndx < countOfCovisibleObservations; ++covisibleIndx)
            {
                // находим совидимый новому узлу узел
                int covisibleId = observation.covisibleObservationsIds[covisibleIndx]; 
                SlamObservationNode covisibleObservationNode = m_slamObservationNodes[covisibleId];

                // совидимому узлу добавляем новый узел и индекс грани соединения с ним
                covisibleObservationNode.NodeLineIDPair.Add(newNode, linesIds[covisibleIndx]);

                // новому узлу добавляем совидимый узел и индекс грани соединения с ним
                newNode.NodeLineIDPair.Add(covisibleObservationNode, linesIds[covisibleIndx]);

                // рисуем грань
                m_fastLinesCloud.SetLine(linesIds[covisibleIndx], newNode.Position, covisibleObservationNode.Position, Color.red);
            }
            m_slamObservationNodes.Add(observation.id, newNode);
        }

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
                m_fastLinesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0)); // удаляем грань
                covisibleNode.NodeLineIDPair.Remove(observationToRemove);
            }

            MF_AutoPool.Despawn(observationToRemove.ObservationObject);
        }

        /// <summary>
        /// Переместить Observation
        /// </summary>
        /// <param name="observation">Observation с абсолютными координатами</param>
        public void ReplaceObservation(SlamObservation observation)
        {
            // находим узел, который необходимо переместить
            SlamObservationNode nodeToReplace = m_slamObservationNodes[observation.id];

            foreach (var covisibleNode in nodeToReplace.NodeLineIDPair)
            {
                // получаем грань, которую необходимо изменить
                Vector3 position1;
                Vector3 position2;
                Color color;
                m_fastLinesCloud.GetLine(covisibleNode.Value, out position1, out position2, out color);

                // определяем торцы изменённой грани
                // если позиция совпадает, то это торец, который нужно поменять
                position1 = position1 == nodeToReplace.Position ? observation.position : position1;
                position2 = position2 == nodeToReplace.Position ? observation.position : position2;

                // записываем изменение в облако
                m_fastLinesCloud.SetLinePosition(covisibleNode.Value, position1, position2);
            }

            // Запоминаем текущую позицию
            nodeToReplace.Position = observation.position;

            // Перемещаем в сцене
            nodeToReplace.ObservationObject.transform.position = observation.position;
            nodeToReplace.ObservationObject.transform.rotation = observation.orientation;

            nodeToReplace.SlamObservation = observation;
        }

        public SlamObservation GetObservationNode(int observationId)
        {
            return m_slamObservationNodes[observationId].SlamObservation;
        }

        public bool ObservationExists(int observationId)
        {
            return m_slamObservationNodes.ContainsKey(observationId);
        }

        public void RepaintGraph()
        {
            m_fastLinesCloud.Repaint();
        }

        public void Clear()
        {
            m_slamObservationNodes.Clear();
            m_fastLinesCloud.Clear();
            MF_AutoPool.DespawnPool(observationPrefab);
        }
    }
}
