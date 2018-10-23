﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class ClearCommand : ISlamEventCommand
    {
        SlamPointsContainer m_pointsContainer;
        SlamLinesContainer m_linesContainer;
        SlamObservationsGraph m_graph;

        SlamLine[] m_undoLines;
        SlamPoint[] m_undoPoints;
        SlamObservation[] m_undoObservations;

        public ClearCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_graph = graph;

            m_undoLines = m_linesContainer.GetAllSlamLines();
            m_undoPoints = m_pointsContainer.GetAllSlamPoints();
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
