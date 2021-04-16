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
    }
}