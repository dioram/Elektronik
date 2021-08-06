using Elektronik.Data.Converters;
using UnityEngine;

namespace Elektronik.RosPlugin.Common
{
    public class RosConverter : ICSConverter
    {
        private Vector3 _scale = Vector3.one;

        public void SetInitTRS(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            _scale = scale;
        }

        public void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            Convert(ref pos);
        }

        public void Convert(ref Vector3 pos)
        {
            pos.x *= _scale.x;
            var tmp = pos.y;
            pos.y = pos.z * _scale.y;
            pos.z = tmp * _scale.z;
        }

        public Vector3 Convert(Vector3 pos)
        {
            Convert(ref pos);
            return pos;
        }

        public Quaternion Convert(Quaternion rot)
        {
            return rot;
        }

        public (Vector3 pos, Quaternion rot) Convert(Vector3 pos, Quaternion rot)
        {
            Convert(ref pos, ref rot);
            return (pos, rot);
        }

        public void ConvertBack(ref Vector3 pos)
        {
            pos.x /= _scale.x;
            var tmp = pos.y;
            pos.y = pos.z / _scale.y;
            pos.z = tmp / _scale.z;
        }

        public void ConvertBack(ref Vector3 pos, ref Quaternion rot)
        {
            ConvertBack(ref pos);
        }
    }
}