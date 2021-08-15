using UnityEngine;

namespace Elektronik.Data.Converters
{
    public class Camera2Unity3dPackageConverter : CSConverter
    {
        private Matrix4x4 _initPose;
        private Vector3 _scale = Vector3.one;

        public override void Convert(ref Vector3 pos)
        {
            pos.x *= _scale.x;
            pos.y = -pos.y * _scale.y;
            pos.z *= _scale.z;
        }

        public override Vector3 Convert(Vector3 pos)
        {
            Convert(ref pos);
            return pos;
        }

        public override Quaternion Convert(Quaternion rot)
        {
            rot.y = -rot.y;
            rot.w = -rot.w;
            return rot;
        }

        public override (Vector3 pos, Quaternion rot) Convert(Vector3 pos, Quaternion rot)
        {
            Convert(ref pos, ref rot);
            return (pos, rot);
        }

        public override void ConvertBack(ref Vector3 pos)
        {
            pos.x /= _scale.x;
            pos.y = -pos.y / _scale.y;
            pos.z /= _scale.z;
        }

        public override void ConvertBack(ref Vector3 pos, ref Quaternion rot)
        {
            pos.y = -pos.y;
            rot.y = -rot.y;
            rot.w = -rot.w;
            Matrix4x4 curHomo = Matrix4x4.TRS(pos, rot, Vector3.one);
            curHomo = _initPose.inverse * curHomo;
            pos = curHomo.GetColumn(3);
            rot = Quaternion.LookRotation(curHomo.GetColumn(2), curHomo.GetColumn(1));
        }

        public override void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            _scale = scale;
            _initPose = Matrix4x4.TRS(pos, rot, scale);
        }

        public override void SetInitTRS(Vector3 pos, Quaternion rot) => SetInitTRS(pos, rot, Vector3.one);

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