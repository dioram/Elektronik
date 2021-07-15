using System;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshUpdatedEventArgs : EventArgs
    {
        public readonly Vector3[] Vertices;
        public readonly Vector3[] Normals;
        public readonly int[] Triangles;

        public MeshUpdatedEventArgs(Vector3[] vertices, Vector3[] normals, int[] triangles)
        {
            Vertices = vertices;
            Normals = normals;
            Triangles = triangles;
        }
    }
}