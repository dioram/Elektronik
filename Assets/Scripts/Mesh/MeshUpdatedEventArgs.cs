using System;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshUpdatedEventArgs : EventArgs
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;

        public MeshUpdatedEventArgs(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}