using Elektronik.Common;
using Elektronik.Offline.Events;
using Elektronik.Offline.SlamEventsCommandPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public class RepaintLinesCommand : ISlamEventCommand
    {
        FastPointCloud m_pointCloud;
        FastLinesCloud m_linesCloud;

        SlamLine[] m_operand;
        SlamLine[] m_undoOperand;

        public RepaintLinesCommand(FastLinesCloud linesCloud, FastPointCloud pointCloud, SlamLine[] lines)
        {
            m_pointCloud = pointCloud;
            m_linesCloud = linesCloud;
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
                int lineId = FastLinesCloud.GetIdxFor2VertIds(operand[i].pointId1, operand[i].pointId2);
                if (operand[i].isRemoved)
                {
                    m_linesCloud.SetLine(lineId, Vector3.zero, Vector3.zero, new Color(0, 0, 0, 0));
                }
                else
                {
                    Vector3 point1pos;
                    Color point1col;
                    Vector3 point2pos;
                    Color point2col;
                    m_pointCloud.GetPoint(operand[i].pointId1, out point1pos, out point1col);
                    m_pointCloud.GetPoint(operand[i].pointId2, out point2pos, out point2col);
                    m_linesCloud.SetLine(lineId, point1pos, point2pos, operand[i].color);
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
