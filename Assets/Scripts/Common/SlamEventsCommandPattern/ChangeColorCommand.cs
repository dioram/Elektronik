using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class ChangeColorCommand : ISlamEventCommand
    {
        private SlamPoint[] m_points2BackColor;

        private SlamPoint[] m_points2ChangeColor;

        private SlamPointsContainer m_pointsContainer;

        public ChangeColorCommand(SlamPointsContainer pointsContainer, ISlamEvent slamEvent)
        {
            m_pointsContainer = pointsContainer;

            if (slamEvent.Points != null)
            {
                m_points2BackColor = slamEvent.Points.Where(p => p.id != -1).Select(p => pointsContainer.GetPoint(p.id)).ToArray();
            
                // Пока раскрашиваются только точки
                //m_lines2BackColor = slamEvent.Lines.Select(l => linesContainer.GetLine(l)).ToArray();
                //m_observations2BackColor = slamEvent.Observations.Select(o => graph.GetObservationNode(o.id)).ToArray();

                m_points2ChangeColor = slamEvent.Points.Where(p => p.id != -1).ToArray();

                //m_lines2ChangeColor = slamEvent.Lines;
                //m_observations2ChangeColor = slamEvent.Observations;
            }
        }

        public void Execute()
        {
            if (m_points2ChangeColor != null)
            {
                foreach (var point in m_points2ChangeColor)
                {
                    m_pointsContainer.ChangeColor(point);
                }
            }
        }

        public void UnExecute()
        {
            if (m_points2BackColor != null)
            {
                foreach (var point in m_points2BackColor)
                {
                    m_pointsContainer.ChangeColor(point);
                }
            }
        }
    }
}
