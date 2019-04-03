using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public class Camera2Unity3dPackageConverter : IPackageCSConverter
    {
        Matrix4x4 m_initPose;

        public Camera2Unity3dPackageConverter(Matrix4x4 initPose)
        {
            m_initPose = initPose;
        }

        public void Convert(ref Package package)
        {
            if (package.Observations != null)
            {
                for (int observationIdx = 0; observationIdx < package.Observations.Count; ++observationIdx)
                {
                    Vector3 curPos = package.Observations[observationIdx].Point.position;
                    Quaternion curRot = package.Observations[observationIdx].Orientation.normalized;
                    curPos.y = -curPos.y;
                    curRot.y = -curRot.y; curRot.w = -curRot.w;

                    Matrix4x4 curHomo = Matrix4x4.TRS(curPos, curRot, Vector3.one);

                    curHomo = m_initPose * curHomo;
                    curPos = curHomo.GetColumn(3);
                    curRot = Quaternion.LookRotation(curHomo.GetColumn(2), curHomo.GetColumn(1));

                    SlamPoint obsPoint = package.Observations[observationIdx];
                    obsPoint.position = curPos;
                    package.Observations[observationIdx].Point = obsPoint;
                    package.Observations[observationIdx].Orientation = curRot;
                }
            }

            if (package.Points != null)
            {
                for (int pointIdx = 0; pointIdx < package.Points.Count; ++pointIdx)
                {
                    Vector3 pointPosition = package.Points[pointIdx].position;
                    pointPosition.y = -pointPosition.y;
                    SlamPoint point = package.Points[pointIdx];
                    point.position = (m_initPose * Matrix4x4.Translate(pointPosition)).GetColumn(3);
                    package.Points[pointIdx] = point;
                }
            }
        }
    }
}
