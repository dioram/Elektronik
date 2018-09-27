using Elektronik.Common;
using Elektronik.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class RepaintLinesCommand : ISlamEventCommand
    {
        SlamPointsContainer m_pointsContainer;
        SlamLinesContainer m_linesContainer;

        SlamLine[] m_operand;
        SlamLine[] m_undoOperand;

        public RepaintLinesCommand(SlamPointsContainer m_pointsContainer, SlamLinesContainer linesContainer, SlamLine[] lines)
        {
            m_linesContainer = linesContainer;
            m_operand = GetOperand(lines);
            m_undoOperand = GetUndoOperand(lines);
        }

        private SlamLine[] GetOperand(SlamLine[] operand)
        {
            return operand.Clone() as SlamLine[];
        }

        private SlamLine[] GetUndoOperand(SlamLine[] operand)
        {
            SlamLine[] result = new SlamLine[operand.Length];
            for (int i = 0; i < operand.Length; ++i)
            {
                result[i] = operand[i];
                result[i].isRemoved = !operand[i].isRemoved;
            }
            return result;
        }

        private void SetOperand(SlamLine[] operand)
        {
            for (int i = 0; i < operand.Length; ++i)
            {
                //int lineId = FastLinesCloud.GetIdxFor2VertIds(operand[i].pointId1, operand[i].pointId2);

                /*if (operand[i].isRemoved)
                {
                    m_linesContainer.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                }
                else
                {
                    Vector3 point1pos;
                    Color point1col;
                    Vector3 point2pos;
                    Color point2col;
                    m_pointCloud.GetPoint(operand[i].pointId1, out point1pos, out point1col);
                    m_pointCloud.GetPoint(operand[i].pointId2, out point2pos, out point2col);
                    m_linesContainer.SetLine(lineId, point1pos, point2pos, operand[i].color);
                }*/
                if (operand[i].isRemoved)
                {
                    m_linesContainer.Remove(operand[i]);
                }
                else
                {
                    operand[i].vert1 = m_pointsContainer.GetPoint(operand[i].pointId1).position;
                    operand[i].vert2 = m_pointsContainer.GetPoint(operand[i].pointId2).position;
                    m_linesContainer.SetLine(operand[i]);
                }
            }
        }

        public void Execute()
        {
            SetOperand(m_operand);
        }

        public void UnExecute()
        {
            SetOperand(m_undoOperand);
        }
    }
}
