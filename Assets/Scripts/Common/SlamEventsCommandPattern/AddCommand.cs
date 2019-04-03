using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class AddCommand : ISlamEventCommand
    {
        private SlamLine[] m_addedLines;
        private readonly SlamPoint[] m_addedPoints;
        private readonly SlamObservation[] m_addedObservations;

        private ISlamContainer<SlamObservation> m_graph;
        private ISlamContainer<SlamLine> m_linesContainer;
        private ISlamContainer<SlamPoint> m_pointsContainer;

        public AddCommand(ISlamContainer<SlamPoint> pointsContainer, ISlamContainer<SlamLine> linesContainer, ISlamContainer<SlamObservation> graph, Package slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_graph = graph;

            if (slamEvent.Points != null)
            {
                m_addedPoints = slamEvent.Points
                    .Where(p => p.isNew)
                    .ToArray();
                
            }
            if (slamEvent.Lines != null)
            {
                m_addedLines = slamEvent.Lines.Where(l => !m_linesContainer.Exists(l)).ToArray();
                for (int i = 0; i < m_addedLines.Length; ++i)
                {
                    m_addedLines[i].isRemoved = true;
                    m_addedLines[i].vert1 = m_pointsContainer.Get(m_addedLines[i].pointId1).position;
                    m_addedLines[i].vert2 = m_pointsContainer.Get(m_addedLines[i].pointId2).position;
                }
            }
            if (slamEvent.Observations != null)
            {
                m_addedObservations = slamEvent.Observations
                    .Where(o => !m_graph.Exists(o.Point.id) && o.Point.id != -1)
                    .Select(o => new SlamObservation(o))
                    .ToArray();
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
                    m_graph.Add(new SlamObservation(observation, false));
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
                    m_graph.Remove(observation.Point.id);
                }
            }
        }
    }
}
