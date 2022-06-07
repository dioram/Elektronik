using Elektronik.Plugins.Common;
using UnityEngine;

namespace Elektronik.RosPlugin.Common
{
    public class RosConverter : ICSConverter
    {
        /// <inheritdoc />
        public void Convert(ref Vector3 pos, ref Quaternion rot)
        {
            Convert(ref pos);
        }

        /// <inheritdoc />
        public void Convert(ref Vector3 pos)
        {
            (pos.y, pos.z) = (pos.z, pos.y);
        }

        /// <inheritdoc />
        public void Convert(ref Quaternion rot)
        {
            // Do nothing
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
            (pos.y, pos.z) = (pos.z, pos.y);
        }

        void ICSConverter.ConvertBack(ref Quaternion rot)
        {
            // Do nothing
        }

        public (Vector3 pos, Quaternion rot) ConvertedBack(Vector3 pos, Quaternion rot)
        {
            (pos.y, pos.z) = (pos.z, pos.y);
            return (pos, rot);
        }

        public Vector3 ConvertedBack(Vector3 pos)
        {
            (pos.y, pos.z) = (pos.z, pos.y);
            return pos;
        }

        public Quaternion ConvertedBack(Quaternion rot)
        {
            return rot;
        }

        /// <inheritdoc />
        public void ConvertBack(ref Vector3 pos, ref Quaternion rot)
        {
            ConvertBack(ref pos);
        }
    }
}