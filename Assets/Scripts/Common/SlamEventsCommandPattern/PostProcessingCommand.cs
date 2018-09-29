using Elektronik.Common;
using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class PostProcessingCommand : MacroCommand
    {
        public PostProcessingCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamObservationsGraph graph, ISlamEvent slamEvent)
        {
            SlamPoint[] points;
            if (slamEvent.Points != null)
            {
                points = slamEvent.Points.Where(p => p.id != -1).ToArray();
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i].color = Color.black;
                }
                m_commands.Add(new ChangeColorCommand(pointsContainer, points));
                m_commands.Add(new RemoveCommand(pointsContainer, linesContainer, graph, slamEvent));
            }
            
        }

        /*private SlamLine[] m_lines;
        private SlamLinesContainer m_linesContainer;

        private SlamPoint[] m_points;
        private SlamPointsContainer m_pointsContainer;

        public PostProcessingCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, ISlamEvent slamEvent)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            if (slamEvent.Points != null)
            {
                m_points = GetPoints(slamEvent.Points.Where(p => p.id != -1).ToArray());
                m_commands.Add(new RepaintPointsCommand(pointsContainer, m_points));
            }
            if (slamEvent.Lines != null)
            {
                m_lines = GetLines(slamEvent.Lines);
                m_commands.Add(new RepaintLinesCommand(pointsContainer, linesContainer, m_lines));
            }
        }

        private SlamLine[] GetLines(SlamLine[] lines)
        {
            SlamLine[] dstLines = new SlamLine[lines.Length];
            for (int i = 0; i < dstLines.Length; ++i)
            {
                dstLines[i] = lines[i];
                dstLines[i].isRemoved = true;
            }
            return dstLines;
        }

        private SlamPoint[] GetPoints(SlamPoint[] points)
        {
            SlamPoint[] dstPoints = new SlamPoint[points.Length];
            for (int i = 0; i < dstPoints.Length; ++i)
            {
                dstPoints[i] = points[i];
                dstPoints[i].color = Color.black;
            }
            return dstPoints;
        }

        private void RemoveLines()
        {
            if (m_lines != null)
            {
                for (int i = 0; i < m_lines.Length; ++i)
                {
                    if (m_lines[i].isRemoved)
                    {
                        m_linesContainer.Remove(m_lines[i]);
                    }
                }
            }
        }

        private void RemovePoints()
        {
            if (m_points != null)
            {
                for (int i = 0; i < m_points.Length; ++i)
                {
                    if (m_points[i].isRemoved)
                    {
                        m_pointsContainer.Remove(m_points[i].id);
                    }
                }
            }
        }

        private void GetBackPoints()
        {
            if (m_points != null)
            {
                for (int i = 0; i < m_points.Length; ++i)
                {
                    if (m_points[i].isRemoved)
                    {
                        m_pointsContainer.Add(m_points[i]);
                    }
                }
            }
        }

        private void GetBackLines()
        {
            if (m_lines != null)
            {
                for (int i = 0; i < m_lines.Length; ++i)
                {
                    if (m_lines[i].isRemoved)
                    {
                        m_linesContainer.Add(m_lines[i]);
                    }
                }
            }
        }

        public override void UnExecute()
        {
            GetBackLines();
            GetBackPoints();
            base.UnExecute();
        }

        public override void Execute()
        {
            Debug.Log("Post processing execute");
            base.Execute();
            RemovePoints();
            RemoveLines();
        }*/
    }
}
