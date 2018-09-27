using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.SlamEventsCommandPattern
{
    public class RepaintObservationsCommand : ISlamEventCommand
    {
        SlamObservationsGraph m_graph;
        
        SlamObservation[] m_operand;
        SlamObservation[] m_undoOperand;

        public RepaintObservationsCommand(SlamObservationsGraph graph, SlamObservation[] operand)
        {
            m_graph = graph;
            m_operand = GetOperand(operand);
            m_undoOperand = GetUndoOperand(operand);
        }

        private SlamObservation[] GetUndoOperand(SlamObservation[] operand)
        {
            SlamObservation[] result = new SlamObservation[operand.Length];

            for (int i = 0; i < result.Length; ++i)
            {
                if (m_graph.ObservationExists(operand[i].id))
                {
                    result[i] = new SlamObservation(m_graph.GetObservationNode(operand[i].id));
                }
                else
                {
                    result[i] = new SlamObservation(operand[i]);
                    result[i].isRemoved = true;
                }
            }
            return result;
        }

        private SlamObservation[] GetOperand(SlamObservation[] operand)
        {
            SlamObservation[] result = new SlamObservation[operand.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new SlamObservation(operand[i]);
                /*if (m_graph.ObservationExists(result[i].id) && result[i].id != -1)
                {
                    SlamObservation currentObservation = m_graph.GetObservationNode(result[i].id);
                    Matrix4x4 currentOrientation = Matrix4x4.TRS(currentObservation.position, currentObservation.orientation.normalized, Vector3.one);
                    Matrix4x4 relativeOrientation = Matrix4x4.TRS(operand[i].position, operand[i].orientation.normalized, Vector3.one);
                    Matrix4x4 newOrientation = currentOrientation * relativeOrientation.inverse;
                    result[i].position = newOrientation.GetColumn(3);
                    result[i].orientation = Quaternion.LookRotation(newOrientation.GetColumn(2), newOrientation.GetColumn(1));
                }*/
            }
            return result;
        }

        private void ChangeGraph(SlamObservation[] operand)
        {
            foreach (var op in operand)
            {
                //m_graph.UpdateOrAddObservation(op);
                if (!m_graph.ObservationExists(op.id)) // новый observation
                {
                    m_graph.AddNewObservation(op);
                }
                else if (op.isRemoved == true) // удалённый observation
                {
                    m_graph.RemoveObservation(op.id);
                }
                else
                {
                    m_graph.ReplaceObservation(op);
                }
            }
        }

        public void Execute()
        {
            ChangeGraph(m_operand);
        }

        public void UnExecute()
        {
            ChangeGraph(m_undoOperand);
        }
    }
}
