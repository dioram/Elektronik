using Elektronik.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline.SlamEventsCommandPattern
{
    public class RepaintPointsCommand : ISlamEventCommand
    {
        FastPointCloud m_pointCloudManager;
        SlamPoint[] m_operand;
        SlamPoint[] m_undoOperand;

        public RepaintPointsCommand(FastPointCloud pointCloudManager, SlamPoint[] operand)
        {
            m_pointCloudManager = pointCloudManager;
            m_operand = GetOperand(operand);
            m_undoOperand = GetUndoOperand(m_operand);
        }

        public SlamPoint[] GetOperand(SlamPoint[] operand)
        {
            SlamPoint[] points = new SlamPoint[operand.Length];
            for (int i = 0; i < points.Length; ++i)
            {
                points[i].id = operand[i].id;
                points[i].isRemoved = operand[i].isRemoved;
                m_pointCloudManager.GetPoint(points[i].id, out points[i].position, out points[i].color);
                points[i].position += operand[i].position;
                points[i].color = operand[i].color;
            }
            return points;
        }

        public SlamPoint[] GetUndoOperand(SlamPoint[] operand)
        {
            SlamPoint[] result = new SlamPoint[operand.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = operand[i];
                m_pointCloudManager.GetPoint(result[i].id, out result[i].position, out result[i].color);
                if (result[i].color == new Color(0, 0, 0, 0))
                    result[i].isRemoved = true;
            }
            return result;
        }

        public void Execute()
        {
            for (int i = 0; i < m_operand.Length; ++i)
            {
                m_pointCloudManager.SetPoint(m_operand[i].id, m_operand[i].position, m_operand[i].color);
            }
        }

        public void UnExecute()
        {
            for (int i = 0; i < m_operand.Length; ++i)
            {
                m_pointCloudManager.SetPoint(m_undoOperand[i].id, m_undoOperand[i].position, m_undoOperand[i].color);
            }
        }
    }
}
