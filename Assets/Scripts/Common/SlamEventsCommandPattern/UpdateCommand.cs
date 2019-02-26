using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using Elektronik.Offline;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class UpdateCommand : ISlamEventCommand
    {
        private SlamObservation[] m_observations2Restore;
        private SlamObservation[] m_observations2Update;

        private SlamPoint[] m_points2Restore;
        private SlamPoint[] m_points2Update;

        private ISlamContainer<SlamPoint> m_pointsContainer;
        private SlamObservationsGraph m_graph;
        private Helmet m_helmet;
        private SlamObservation m_helmetPose;


        public UpdateCommand(
            ISlamContainer<SlamPoint> pointsContainer,
            SlamObservationsGraph graph,
            Helmet helmet,
            Package slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_graph = graph;
            m_helmet = helmet;

            if (slamEvent.Points != null)
            {
                m_points2Restore = slamEvent.Points.Where(p => p.id != -1).Select(p => pointsContainer.Get(p.id)).ToArray();
                m_points2Update = slamEvent.Points.Where(p => p.id != -1).ToArray();
            }

            if (slamEvent.Observations != null)
            {
                m_helmetPose = slamEvent.Observations.FirstOrDefault(o => o.id == -1);
                m_observations2Restore = slamEvent.Observations
                    .Where(o => o.id != -1)
                    .Where(o => !o.isRemoved)
                    .Select(o => new SlamObservation(graph.Get(o.id))).ToArray();
                m_observations2Update = slamEvent.Observations
                    .Where(o => o.id != -1)
                    .Where(o => !o.isRemoved)
                    .Select(o => new SlamObservation(o)).ToArray();
            }
        }

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

            if (m_helmetPose != null)
            {
                m_helmet.ReplaceAbs(m_helmetPose.position, m_helmetPose.orientation);
            }

            if (m_observations2Update != null)
            {
                foreach (var observation in m_observations2Update)
                {
                    m_graph.Replace(observation);
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

            if (m_helmetPose != null)
            {
                m_helmet.TurnBack();
            }

            if (m_observations2Restore != null)
            {
                foreach (var observation in m_observations2Restore)
                {
                    
                     m_graph.Replace(observation);
                    
                }
            }
        }
    }
}
