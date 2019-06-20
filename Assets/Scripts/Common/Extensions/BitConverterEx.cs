
using System;
using UnityEngine;

namespace Elektronik.Common.Extensions
{
    public static class BitConverterEx
    {
        public static readonly bool IsLittleEndian;
        static BitConverterEx() => IsLittleEndian = BitConverter.IsLittleEndian;
        public static long DoubleToInt64Bits(double value) => BitConverter.DoubleToInt64Bits(value);
        public static byte[] GetBytes(bool value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(char value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(double value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(short value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(int value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(long value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(float value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(ushort value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(uint value) => BitConverter.GetBytes(value);
        public static byte[] GetBytes(ulong value) => BitConverter.GetBytes(value);
        public static double Int64BitsToDouble(long value) => BitConverter.Int64BitsToDouble(value);
        public static bool ToBoolean(byte[] value, int startIndex) => BitConverter.ToBoolean(value, startIndex);
        public static bool ToBoolean(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(bool);
            return ToBoolean(value, startIndex);
        }
        public static char ToChar(byte[] value, int startIndex) => BitConverter.ToChar(value, startIndex);
        public static char ToChar(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(char);
            return ToChar(value, startIndex);
        }
        public static double ToDouble(byte[] value, int startIndex) => BitConverter.ToDouble(value, startIndex);
        public static double ToDouble(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(double);
            return ToDouble(value, startIndex);
        }
        public static short ToInt16(byte[] value, int startIndex) => BitConverter.ToInt16(value, startIndex);
        public static short ToInt16(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(short);
            return ToInt16(value, startIndex);
        }
        public static int ToInt32(byte[] value, int startIndex) => BitConverter.ToInt32(value, startIndex);
        public static int ToInt32(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(int);
            return ToInt32(value, startIndex);
        }
        public static long ToInt64(byte[] value, int startIndex) => BitConverter.ToInt64(value, startIndex);
        public static long ToInt64(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(long);
            return ToInt64(value, startIndex);
        }
        public static float ToSingle(byte[] value, int startIndex) => BitConverter.ToSingle(value, startIndex);
        public static float ToSingle(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(float);
            return ToSingle(value, startIndex);
        }
        public static string ToString(byte[] value) => BitConverter.ToString(value);
        public static string ToString(byte[] value, int startIndex) => BitConverter.ToString(value, startIndex);
        public static string ToString(byte[] value, int startIndex, int length) => BitConverter.ToString(value, startIndex, length);
        public static ushort ToUInt16(byte[] value, int startIndex) => BitConverter.ToUInt16(value, startIndex);
        public static ushort ToUInt16(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(ushort);
            return ToUInt16(value, startIndex);
        }
        public static uint ToUInt32(byte[] value, int startIndex) => BitConverter.ToUInt32(value, startIndex);
        public static uint ToUInt32(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(uint);
            return ToUInt32(value, startIndex);
        }
        public static ulong ToUInt64(byte[] value, int startIndex) => BitConverter.ToUInt64(value, startIndex);
        public static ulong ToUInt64(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(ulong);
            return ToUInt64(value, startIndex);
        }
        public static Vector3 ToVector3(byte[] value, int startIndex) =>
            new Vector3()
            {
                x = BitConverter.ToSingle(value, startIndex),
                y = BitConverter.ToSingle(value, startIndex + sizeof(float)),
                z = BitConverter.ToSingle(value, startIndex + sizeof(float) * 2)
            };
        public static Vector3 ToVector3(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(float) * 3;
            return ToVector3(value, startIndex);
        }
        public static Quaternion ToQuaternion(byte[] value, int startIndex) => new Quaternion()
        {
            w = BitConverter.ToSingle(value, startIndex),
            x = BitConverter.ToSingle(value, startIndex + sizeof(float)),
            y = BitConverter.ToSingle(value, startIndex + sizeof(float) * 2),
            z = BitConverter.ToSingle(value, startIndex + sizeof(float) * 3)
        };
        public static Quaternion ToQuaternion(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(float) * 4;
            return ToQuaternion(value, startIndex);
        }
        public static Color ToRGBColor(byte[] value, int startIndex)
        {
            Color color = new Color()
            {
                r = value[startIndex],
                g = value[startIndex + 1],
                b = value[startIndex + 2],
                a = 0xff
            };
            if (color.a == 1.0 || color.r == 1.0 || color.g == 1.0 || color.b == 1.0)
                Debug.LogWarning("[BitConverterEx.ToRGBColor] One of color channels equals to 1.0, are you sure? Max value of this color is 255.");
            return color;
        }
        public static Color ToRGBColor(byte[] value, int startIndex, ref int offset)
        {
            offset += sizeof(byte) * 3;
            return ToRGBColor(value, startIndex);
        }
    }
}