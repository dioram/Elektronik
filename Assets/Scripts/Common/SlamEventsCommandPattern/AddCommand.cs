using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class AddCommand : ISlamEventCommand
    {
        private SlamLine[] m_addedLines;
        private SlamPoint[] m_addedPoints;
        private SlamObservation[] m_addedObservations;

        private SlamObservationsGraph m_graph;
        private SlamLinesContainer m_linesContainer;
        private SlamPointsContainer m_pointsContainer;

        public AddCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph, ISlamEvent slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_graph = graph;

            if (slamEvent.Points != null)
            {
                m_addedPoints = slamEvent.Points.Where(p => p.id != -1).Where(p => !pointsContainer.PointExists(p.id)).ToArray();
            }
            if (slamEvent.Lines != null)
            {
                m_addedLines = slamEvent.Lines.Where(l => !m_linesContainer.LineExists(l)).ToArray();
            }
            if (slamEvent.Observations != null)
            {
                m_addedObservations = slamEvent.Observations.Where(o => !m_graph.ObservationExists(o.id)).ToArray();
            }
        }

        public void Execute()
        {
            if (m_addedPoints != null)
            {
                foreach (var point in m_addedPoints)
                {
                    m_pointsContainer.Add(point);
                }
            }
            if (m_addedLines != null)
            {
                foreach (var line in m_addedLines)
                {
                    m_linesContainer.Add(line);
                }
            }
            if (m_addedObservations != null)
            {
                foreach (var observation in m_addedObservations)
                {
                    m_graph.AddNewObservation(observation);
                }
            }
        }

        public void UnExecute()
        {
            if (m_addedPoints != null)
            {
                foreach (var point in m_addedPoints)
                {
                    m_pointsContainer.Remove(point.id);
                }
            }
            if (m_addedLines != null)
            {
                foreach (var line in m_addedLines)
                {
                    m_linesContainer.Remove(line);
                }
            }
            if (m_addedObservations != null)
            {
                foreach (var observation in m_addedObservations)
                {
                    m_graph.RemoveObservation(observation.id);
                }
            }
        }
    }
}
