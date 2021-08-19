using System;
using UnityEngine;

namespace Elektronik.Mesh
{
    public class MeshUpdatedEventArgs : EventArgs
    {
        public readonly (Vector3 pos, Color color)[] Vertices;
        public readonly int[] Triangles;

        public MeshUpdatedEventArgs((Vector3 pos, Color color)[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}