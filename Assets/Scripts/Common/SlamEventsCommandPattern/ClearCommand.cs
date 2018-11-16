using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Common.Containers;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class ClearCommand : ISlamEventCommand
    {
        ISlamContainer<SlamPoint> m_pointsContainer;
        ISlamContainer<SlamLine> m_linesContainer;
        SlamObservationsGraph m_graph;

        SlamLine[] m_undoLines;
        SlamPoint[] m_undoPoints;
        SlamObservation[] m_undoObservations;

        public ClearCommand(ISlamContainer<SlamPoint> pointsContainer, ISlamContainer<SlamLine> linesContainer, SlamObservationsGraph graph)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_graph = graph;

            m_undoLines = m_linesContainer.GetAll();
            m_undoPoints = m_pointsContainer.GetAll();
            m_undoObservations = m_graph.GetAll().Select(o => new SlamObservation(o)).ToArray();
        }

        public void Execute()
        {
            Debug.Log("[Clear Execute]");
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            m_graph.Clear();
        }

        public void UnExecute()
        {
            Debug.Log("[Clear UnExecute]");
            m_pointsContainer.AddRange(m_undoPoints);
            m_linesContainer.AddRange(m_undoLines);
            foreach (var undoObservation in m_undoObservations)
            {
                m_graph.Add(undoObservation);
            }
        }
    }
}
