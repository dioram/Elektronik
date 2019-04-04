using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class ClearCommand : ISlamEventCommand
    {
        ISlamContainer<SlamPoint> m_pointsContainer;
        ISlamContainer<SlamLine> m_linesContainer;
        ISlamContainer<SlamObservation> m_observationsContainer;

        SlamLine[] m_undoLines;
        SlamPoint[] m_undoPoints;
        SlamObservation[] m_undoObservations;

        public ClearCommand(
            ISlamContainer<SlamPoint> pointsContainer,
            ISlamContainer<SlamLine> linesContainer,
            ISlamContainer<SlamObservation> observationsContainer)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_observationsContainer = observationsContainer;

            m_undoLines = m_linesContainer.GetAll();
            m_undoPoints = m_pointsContainer.GetAll();
            m_undoObservations = m_observationsContainer.GetAll();
        }

        public void Execute()
        {
            Debug.Log("[Clear Execute]");
            m_pointsContainer.Clear();
            m_linesContainer.Clear();
            m_observationsContainer.Clear();
        }

        public void UnExecute()
        {
            Debug.Log("[Clear UnExecute]");
            m_pointsContainer.AddRange(m_undoPoints);
            m_linesContainer.AddRange(m_undoLines);
            m_observationsContainer.AddRange(m_undoObservations);
        }
    }
}
