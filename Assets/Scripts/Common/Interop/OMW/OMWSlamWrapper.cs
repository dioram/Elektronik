using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Elektronik.Common.Interop.OMW
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public float[] a;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    sb.AppendFormat("{0} ", a[i * 3 + j]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4x4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] a;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    sb.AppendFormat("{0} ", a[i * 4 + j]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] a;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] a;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] a;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;
        public int timestamp;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("vec: x: {0}; y: {1}; z: {2}{3}", position.a[0], position.a[1], position.a[2], Environment.NewLine);
            sb.AppendFormat("rot: w: {0};  x: {1}; y: {2}; z: {3}{4}", rotation.a[0], rotation.a[1], rotation.a[2], rotation.a[3], Environment.NewLine);
            return sb.ToString();
        }
    }

    [InitializeOnLoad]
    public static class OMWSlamWrapper
    {
        static OMWSlamWrapper() // static Constructor
        {
            var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
            var omwDepsPath = Environment.GetEnvironmentVariable("OMW_DEPS", EnvironmentVariableTarget.Process);
            if (currentPath != null && !currentPath.Contains(omwDepsPath))
                Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator
                    + omwDepsPath, EnvironmentVariableTarget.Process);
        }

        private const string PATH_TO_DLL = @"OMW.SDK";

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern IntPtr createTracker();

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern void destroyTracker(IntPtr tracker);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool getAbsPose(IntPtr tracker, ref Pose pose);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool getRelPose(IntPtr tracker, ref Pose pose);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool getWorldFromTrackerTransform(IntPtr tracker, ref Matrix4x4 transform);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool getTrackerFromHeadTransform(IntPtr tracker, ref Matrix4x4 transform);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool getEyeToHeadTransform(IntPtr tracker, ref Matrix4x4 transform);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool init(IntPtr tracker);

        [DllImport(PATH_TO_DLL, CharSet = CharSet.Auto)]
        public static extern bool isReadyToUse(IntPtr tracker);
    }
}
