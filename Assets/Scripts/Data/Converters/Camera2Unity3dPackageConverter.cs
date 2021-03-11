using UnityEngine;

namespace Elektronik.Data.Converters
{
    public class Camera2Unity3dPackageConverter : CSConverter
    {
        private Matrix4x4 _initPose;

        public override void Convert(ref Vector3 pos)
        {
            pos.y = -pos.y;
        }

        public override void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            _initPose = Matrix4x4.TRS(pos, rot, scale);
        }

        public override void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            pos.y = -pos.y;
            rot.y = -rot.y;
            rot.w = -rot.w;
            Matrix4x4 curHomo = Matrix4x4.TRS(pos, rot, Vector3.one);
            curHomo = _initPose * curHomo;
            pos = curHomo.GetColumn(3);
            rot = Quaternion.LookRotation(curHomo.GetColumn(2), curHomo.GetColumn(1));
        }
    }
}