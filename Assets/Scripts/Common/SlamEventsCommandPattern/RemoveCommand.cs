using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class RemoveCommand : ISlamEventCommand
    {
        private SlamLine[] m_lines2Remove;
        private SlamPoint[] m_points2Remove;
        private SlamObservation[] m_observations2Remove;

        private SlamObservationsGraph m_graph;
        private ISlamContainer<SlamLine> m_linesContainer;
        private ISlamContainer<SlamPoint> m_pointsContainer;

        public RemoveCommand(ISlamContainer<SlamPoint> pointsContainer, ISlamContainer<SlamLine> linesContainer, SlamObservationsGraph graph, Package slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_graph = graph;

            if (slamEvent.Points != null)
            {
                m_points2Remove = slamEvent.Points.Where(p => p.id != -1).Where(p => p.isRemoved).ToArray();
                for (int i = 0; i < m_points2Remove.Length; ++i)
                {
                    m_points2Remove[i].position = m_pointsContainer.Get(m_points2Remove[i].id).position;
                }
            }
            if (slamEvent.Lines != null)
            {
                m_lines2Remove = slamEvent.Lines.Where(l => l.isRemoved).ToArray();
                for (int i = 0; i < m_lines2Remove.Length; ++i)
                {
                    m_lines2Remove[i].vert1 = m_pointsContainer.Get(m_lines2Remove[i].pointId1).position;
                    m_lines2Remove[i].vert2 = m_pointsContainer.Get(m_lines2Remove[i].pointId2).position;
                }
            }
            if (slamEvent.Observations != null)
            {
                m_observations2Remove = slamEvent.Observations
                    .Where(o => o.id != -1)
                    .Where(o => o.isRemoved)
                    .Select(o => new SlamObservation(o))
                    .ToArray();
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
                    m_graph.Remove(observation.id);
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
                    m_graph.Add(observation);
                }
            }
        }
    }
}
