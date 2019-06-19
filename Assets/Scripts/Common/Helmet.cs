using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common
{
    public class Helmet : MonoBehaviour
    {
        private ICloudObjectsContainer<SlamLine> m_linesContainer;
        private int m_lastLineId;
        private int m_lineSegmentIdx;
        private Stack<Pose> m_poseHistory;
        private Stack<int> m_lineIdsHistory;

        public FastLinesCloud linesCloud;
        public Color color = Color.red;
        public int id;

        public void Start()
        {
            m_lastLineId = -1;
            m_linesContainer = new SlamLinesContainer(linesCloud);
            m_poseHistory = new Stack<Pose>();
            m_lineIdsHistory = new Stack<int>();
        }

        public void TurnBack()
        {
            if (m_poseHistory.Count > 0)
            {
                Pose lastPose = m_poseHistory.Pop();
                transform.position = lastPose.position;
                transform.rotation = lastPose.rotation;
            }
            if (m_lineIdsHistory.Count > 0)
            {
                int lastLineId = m_lineIdsHistory.Pop();
                if (lastLineId != -1)
                    m_linesContainer.Remove(lastLineId);
                m_linesContainer.Repaint();
            }
        }

        private void ContinueTrack(Vector3 vert1, Vector3 vert2)
        {
            if (vert1 == vert2)
            {
                m_lineIdsHistory.Push(-1);
                return;
            }
            SlamLine line = new SlamLine()
            {
                color1 = color,
                isRemoved = false,
                pointId1 = m_lineSegmentIdx,
                pointId2 = ++m_lineSegmentIdx,
                vert1 = vert1,
                vert2 = vert2,
            };
            m_lastLineId = m_linesContainer.Add(line);
            m_lineIdsHistory.Push(m_lastLineId);
            m_linesContainer.Repaint();
        }

        public void ReplaceAbs(Vector3 position, Quaternion rotation)
        {
            m_poseHistory.Push(new Pose(transform.position, transform.rotation));
            ContinueTrack(transform.position, position);
            transform.SetPositionAndRotation(position, rotation);
        }

        public void ReplaceRel(Vector3 position, Quaternion rotation)
        {
            m_poseHistory.Push(new Pose(transform.position, transform.rotation));
            Matrix4x4 current = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Matrix4x4 rel = Matrix4x4.TRS(position, rotation, Vector3.one);
            Matrix4x4 newPose = current * rel;
            Vector3 newPosition = newPose.GetColumn(3);
            Quaternion newRotation = Quaternion.LookRotation(newPose.GetColumn(2), newPose.GetColumn(1));
            ContinueTrack(transform.position, position);
            transform.position = newPosition;
            transform.rotation = newRotation;
        }

        public void ResetHelmet()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            m_lastLineId = -1;
            m_lineSegmentIdx = 0;
            m_lineIdsHistory.Clear();
            m_linesContainer.Clear();
            m_poseHistory.Clear();
        }
    }
}