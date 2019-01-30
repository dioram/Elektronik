using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public static class SlamBitConverter
    {
        public static Vector3 ToVector3(byte[] value, int startIndex)
        {
            float x = BitConverter.ToSingle(value, startIndex);
            float y = BitConverter.ToSingle(value, startIndex + sizeof(float));
            float z = BitConverter.ToSingle(value, startIndex + sizeof(float) * 2);
            return new Vector3(x, y, z);
        }

        public static Quaternion ToQuaternion(byte[] value, int startIndex)
        {
            float w = BitConverter.ToSingle(value, startIndex);
            float x = BitConverter.ToSingle(value, startIndex + sizeof(float));
            float y = BitConverter.ToSingle(value, startIndex + sizeof(float) * 2);
            float z = BitConverter.ToSingle(value, startIndex + sizeof(float) * 3);
            return new Quaternion(x, y, z, w);
        }

        public static Color ToRGBColor(byte[] value, int startIndex)
        {
            return new Color32(value[startIndex], value[startIndex + 1], value[startIndex + 2], 0xff);
        }
    }
}
