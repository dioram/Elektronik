using UnityEngine;

namespace Elektronik.Common.Data.Converters
{
    public class Camera2Unity3dPackageConverter : ICSConverter
    {
        Matrix4x4 m_initPose;

        public Camera2Unity3dPackageConverter(Matrix4x4 initPose)
        {
            m_initPose = initPose;
        }

        public void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            pos.y = -pos.y;
            rot.y = -rot.y; rot.w = -rot.w;
            Matrix4x4 curHomo = Matrix4x4.TRS(pos, rot, Vector3.one);
            curHomo = m_initPose * curHomo;
            pos = curHomo.GetColumn(3);
            rot = Quaternion.LookRotation(curHomo.GetColumn(2), curHomo.GetColumn(1));
        }
    }
}
