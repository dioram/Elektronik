using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elektronik.Common.Events;
using UnityEngine;

namespace Elektronik.Common
{
    public class Camera2Unity3dSlamEventConverter : ISlamEventDataConverter
    {
        Matrix4x4 m_initPose;

        public Camera2Unity3dSlamEventConverter(Matrix4x4 initPose)
        {
            m_initPose = initPose;
        }

        public void Convert(ref ISlamEvent srcEvent)
        {
            if (srcEvent.Observations != null)
            {
                for (int observationIdx = 0; observationIdx < srcEvent.Observations.Length; ++observationIdx)
                {
                    Vector3 curPos = srcEvent.Observations[observationIdx].position;
                    Quaternion curRot = srcEvent.Observations[observationIdx].orientation.normalized;
                    curPos.y = -curPos.y;
                    curRot.y = -curRot.y; curRot.w = -curRot.w;

                    Matrix4x4 curHomo = Matrix4x4.TRS(curPos, curRot, Vector3.one);

                    curHomo = m_initPose * curHomo;
                    curPos = curHomo.GetColumn(3);
                    curRot = Quaternion.LookRotation(curHomo.GetColumn(2), curHomo.GetColumn(1));

                    srcEvent.Observations[observationIdx].position = curPos;
                    srcEvent.Observations[observationIdx].orientation = curRot;
                }
            }

            if (srcEvent.Points != null)
            {
                for (int pointIdx = 0; pointIdx < srcEvent.Points.Length; ++pointIdx)
                {
                    Vector3 pointPosition = srcEvent.Points[pointIdx].position;
                    pointPosition.y = -pointPosition.y;
                    srcEvent.Points[pointIdx].position = (m_initPose * Matrix4x4.Translate(pointPosition)).GetColumn(3);
                }
            }
        }
    }
}
