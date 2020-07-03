using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Maps
{
    public class Helmet : MonoBehaviour
    {
        private IConnectionsContainer<SlamLine> m_linesContainer;
        private int m_lineSegmentIdx;
        private Stack<Pose> m_poseHistory;
        private Stack<SlamLine> m_linesHistory;

        public Color color = Color.red;
        public int id;

        private void Awake()
        {
            m_linesContainer = new SlamLinesContainer(GetComponentInChildren<FastLinesCloud>());
            m_poseHistory = new Stack<Pose>();
            m_linesHistory = new Stack<SlamLine>();
        }

        public void TurnBack()
        {
            if (m_poseHistory.Count > 0)
            {
                Pose lastPose = m_poseHistory.Pop();
                transform.position = lastPose.position;
                transform.rotation = lastPose.rotation;
            }
            if (m_linesHistory.Count > 0)
            {
                SlamLine lastLineId = m_linesHistory.Pop();
                m_linesContainer.Remove(lastLineId);
            }
        }

        private void ContinueTrack(Vector3 vert1, Vector3 vert2)
        {
            var pt1 = new SlamPoint()
            {
                id = m_lineSegmentIdx,
                color = color,
                position = vert1,
            };
            SlamPoint pt2 = pt1;
            pt2.id = ++m_lineSegmentIdx;
            pt2.position = vert2;
            var connection = new SlamLine(pt1, pt2);
            m_linesContainer.Add(connection);
            m_linesHistory.Push(connection);
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
            transform.SetPositionAndRotation(newPosition, newRotation);
        }

        public void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                var currentPose = new Pose(transform.position, transform.rotation);
                if (m_poseHistory.Count != 0)
                {
                    ContinueTrack(m_poseHistory.Peek().position, currentPose.position);
                }
                m_poseHistory.Push(currentPose);
            }
        }

        public void ResetHelmet()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            m_lineSegmentIdx = 0;
            m_linesHistory.Clear();
            m_poseHistory.Clear();
            m_linesContainer.Clear();
        }
    }
}