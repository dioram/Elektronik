using UnityEngine;

namespace Elektronik.Containers.EventArgs
{
    public class MeshUpdatedEventArgs : System.EventArgs
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