using UnityEngine;

namespace Elektronik.Plugins.Common
{
    public class RightHandToLeftHandConverter : ICSConverter
    {
        /// <inheritdoc />
        public void Convert(ref Vector3 pos)
        {
            pos.y = -pos.y;
        }

        public void Convert(ref Quaternion rot)
        {
            rot.y = -rot.y;
            rot.w = -rot.w;
        }

        /// <inheritdoc />
        public Vector3 Converted(Vector3 pos)
        {
            Convert(ref pos);
            return pos;
        }

        /// <inheritdoc />
        public Quaternion Converted(Quaternion rot)
        {
            rot.y = -rot.y;
            rot.w = -rot.w;
            return rot;
        }

        /// <inheritdoc />
        public (Vector3 pos, Quaternion rot) Converted(Vector3 pos, Quaternion rot)
        {
            Convert(ref pos, ref rot);
            return (pos, rot);
        }

        /// <inheritdoc />
        public void ConvertBack(ref Vector3 pos)
        {
            pos.y = -pos.y;
        }

        /// <inheritdoc />
        public void ConvertBack(ref Quaternion rot)
        {
            rot.y = -rot.y;
            rot.w = -rot.w;
        }

        /// <inheritdoc />
        public (Vector3 pos, Quaternion rot) ConvertedBack(Vector3 pos, Quaternion rot)
        {
            pos.y = -pos.y;
            rot.y = -rot.y;
            rot.w = -rot.w;
            return (pos, rot);
        }

        public Vector3 ConvertedBack(Vector3 pos)
        {
            pos.y = -pos.y;
            return pos;
        }

        /// <inheritdoc />
        public Quaternion ConvertedBack(Quaternion rot)
        {
            rot.y = -rot.y;
            rot.w = -rot.w;
            return rot;
        }

        /// <inheritdoc />
        public void ConvertBack(ref Vector3 pos, ref Quaternion rot)
        {
            pos.y = -pos.y;
            rot.y = -rot.y;
            rot.w = -rot.w;
        }

        public void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            pos.y = -pos.y;
            rot.y = -rot.y;
            rot.w = -rot.w;
        }
    }
}