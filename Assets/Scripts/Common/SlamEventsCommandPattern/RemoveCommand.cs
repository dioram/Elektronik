using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class RemoveCommand : ISlamEventCommand
    {
        private SlamLine[] m_lines2Remove;
        private SlamPoint[] m_points2Remove;
        private SlamObservation[] m_observations2Remove;

        private SlamObservationsGraph m_graph;
        private SlamLinesContainer m_linesContainer;
        private SlamPointsContainer m_pointsContainer;

        public RemoveCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph, ISlamEvent slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_graph = graph;

            if (slamEvent.Points != null)
            {
                m_points2Remove = slamEvent.Points.Where(p => p.id != -1).Where(p => p.isRemoved).ToArray();
            }
            if (slamEvent.Lines != null)
            {
                m_lines2Remove = slamEvent.Lines.Where(l => l.isRemoved).ToArray();
            }
            if (slamEvent.Observations != null)
            {
                m_observations2Remove = slamEvent.Observations.Where(o => o.isRemoved).ToArray();
            }
        }

        public void Execute()
        {
            if (m_points2Remove != null)
            {
                foreach (var point in m_points2Remove)
                {
                    m_pointsContainer.Remove(point.id);
                }
            }
            if (m_lines2Remove != null)
            {
                foreach (var line in m_lines2Remove)
                {
                    m_linesContainer.Remove(line);
                }
            }
            if (m_observations2Remove != null)
            {
                foreach (var observation in m_observations2Remove)
                {
                    m_graph.RemoveObservation(observation.id);
                }
            }
        }

        public void UnExecute()
        {
            if (m_points2Remove != null)
            {
                foreach (var point in m_points2Remove)
                {
                    m_pointsContainer.Add(point);
                }
            }

            if (m_lines2Remove != null)
            {
                foreach (var line in m_lines2Remove)
                {
                    m_linesContainer.Add(line);
                }
            }
            if (m_observations2Remove != null)
            {
                foreach (var observation in m_observations2Remove)
                {
                    m_graph.AddNewObservation(observation);
                }
            }
        }
    }
}
