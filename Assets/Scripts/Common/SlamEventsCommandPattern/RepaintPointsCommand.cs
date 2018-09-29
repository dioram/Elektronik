using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class RepaintPointsCommand : ISlamEventCommand
    {
        SlamPointsContainer m_pointsContainer;
        SlamPoint[] m_operand;
        SlamPoint[] m_undoOperand;

        public RepaintPointsCommand(SlamPointsContainer pointsContainer, SlamPoint[] operand)
        {
            m_pointsContainer = pointsContainer;
            m_operand = GetOperand(operand);
            m_undoOperand = GetUndoOperand(m_operand);
        }

        public SlamPoint[] GetOperand(SlamPoint[] operand)
        {
            SlamPoint[] points = new SlamPoint[operand.Length];
            for (int i = 0; i < points.Length; ++i)
            {
                points[i] = operand[i];
                if (points[i].justColored)
                {
                    SlamPoint currentPoint = m_pointsContainer.GetPoint(operand[i].id);
                    points[i].position = currentPoint.position;
                }
                points[i].isRemoved = false; // точки удаляем только в постобработке, в данной команде только красим в красный
            }
            return points;
        }

        public SlamPoint[] GetUndoOperand(SlamPoint[] operand)
        {
            SlamPoint[] result = new SlamPoint[operand.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = operand[i];
                if (!m_pointsContainer.TryGetPoint(operand[i], out result[i]))
                {
                    result[i].isRemoved = true; // если точки не было до добавления, то удаляем её
                }
            }
            return result;
        }

        private void SetPoints(SlamPoint[] points)
        {
            for (int i = 0; i < points.Length; ++i)
            {
                if (points[i].isRemoved)
                {
                    m_pointsContainer.Remove(points[i].id);
                }
                else
                {
                    m_pointsContainer.SetPoint(points[i]);
                }
            }
        }

        public void Execute()
        {
            SetPoints(m_operand);
        }

        public void UnExecute()
        {
            SetPoints(m_undoOperand);
        }
    }
}
