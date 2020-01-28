using System;
using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    [Obsolete]
    public struct SlamLine
    {
        public Vector3 vert1;
        public Vector3 vert2;
        public int pointId1;
        public int pointId2;
        public Color color1;
        public Color color2;
        public bool isRemoved;
        public SlamLine(Vector3 vert1, Vector3 vert2, int pointId1, int pointId2, Color color1, Color color2)
        {
            this.vert1 = vert1;
            this.vert2 = vert2;
            this.pointId1 = pointId1;
            this.pointId2 = pointId2;
            this.color1 = color1;
            this.color2 = color2;
            this.isRemoved = false;
        }

        public long GenerateLongId()
        {
            return GenerateLongId(pointId1, pointId2);
        }

        public static long GenerateLongId(int pointId1, int pointId2)
        {
            return pointId1 * UInt32.MaxValue + pointId2;
        }
    }
}
