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

        public RepaintLinesCommand(SlamPointsContainer pointsContainer, SlamLinesContainer linesContainer, SlamLine[] lines)
        {
            m_pointsContainer = pointsContainer;
            m_linesContainer = linesContainer;
            m_operand = GetOperand(lines);
            m_undoOperand = GetUndoOperand(lines);
        }

        private SlamLine[] GetOperand(SlamLine[] operand)
        {
            SlamLine[] result = new SlamLine[operand.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = operand[i];
                result[i].isRemoved = false; // удаляем линии только в постобработке и при отмене этой команды
            }
            return result;
        }

        private SlamLine[] GetUndoOperand(SlamLine[] operand)
        {
            SlamLine[] result = new SlamLine[operand.Length];
            for (int i = 0; i < operand.Length; ++i)
            {
                result[i] = operand[i];
                result[i].isRemoved = true;
            }
            return result;
        }

        private void SetOperand(SlamLine[] operand)
        {
            for (int i = 0; i < operand.Length; ++i)
            {
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
