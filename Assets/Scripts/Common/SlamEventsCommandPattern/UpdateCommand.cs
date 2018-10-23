using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class UpdateCommand : ISlamEventCommand
    {
        private SlamObservation[] m_observations2Restore;
        private SlamObservation[] m_observations2Update;

        private SlamPoint[] m_points2Restore;
        private SlamPoint[] m_points2Update;

        private SlamPointsContainer m_pointsContainer;
        private SlamObservationsGraph m_graph;
        private Helmet m_helmet;


        public UpdateCommand(
            SlamPointsContainer pointsContainer,
            SlamObservationsGraph graph,
            Helmet helmet,
            SlamPoint[] points,
            SlamObservation[] observations)
        {
            m_pointsContainer = pointsContainer;
            m_graph = graph;
            m_helmet = helmet;

            if (points != null)
            {
                m_points2Restore = points.Where(p => p.id != -1).Select(p => pointsContainer.GetPoint(p.id)).ToArray();
                m_points2Update = points.Where(p => p.id != -1).ToArray();
            }

            if (observations != null)
            {
                m_observations2Restore = observations.Select(o => new SlamObservation(graph.Get(o.id))).ToArray();
                m_observations2Update = observations.Select(o => new SlamObservation(o)).ToArray();
            }
        }

        public UpdateCommand(
            SlamPointsContainer pointsContainer,
            SlamObservationsGraph graph, 
            Helmet helmet, 
            ISlamEvent slamEvent) : this(pointsContainer, graph, helmet, slamEvent.Points, slamEvent.Observations)
        { }

        public void Execute()
        {
            if (m_points2Update != null)
            {
                foreach (var point in m_points2Update)
                {
                    if (point.justColored)
                    {
                        m_pointsContainer.ChangeColor(point); // не требуется менять положение
                    }
                    else
                    {
                        m_pointsContainer.Update(point); // требуется сменить цвет/положение
                    }
                }
            }

            if (m_observations2Update != null)
            {
                foreach (var observation in m_observations2Update)
                {
                    if (observation.id == -1)
                    {
                        m_helmet.ReplaceAbs(observation.position, observation.orientation);
                    }
                    else
                    {
                        m_graph.Replace(observation);
                    }
                }
            }
        }

        public void UnExecute()
        {
            if (m_points2Restore != null)
            {
                foreach (var point in m_points2Restore)
                {
                    m_pointsContainer.Update(point); // восстанавливаем и положение и цвет
                }
            }

            if (m_observations2Restore != null)
            {
                foreach (var observation in m_observations2Restore)
                {
                    if (observation.id == -1)
                    {
                        m_helmet.TurnBack();
                    }
                    else
                    {
                        m_graph.Replace(observation);
                    }
                }
            }
        }
    }
}
