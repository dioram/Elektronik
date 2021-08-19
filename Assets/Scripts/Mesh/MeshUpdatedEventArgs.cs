using System;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshUpdatedEventArgs : EventArgs
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;
        public readonly Color[] Colors;

        public MeshUpdatedEventArgs(Vector3[] vertices, int[] triangles, Color[] colors)
        {
            Vertices = vertices;
            Triangles = triangles;
            Colors = colors;
        }
    }
}