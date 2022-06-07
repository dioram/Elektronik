using System.Linq;
using UnityEngine;

namespace Elektronik.DataSources.Containers.EventArgs
{
    /// <summary> Event args for updating mesh. </summary>
    public class MeshUpdatedEventArgs : System.EventArgs
    {
        public readonly (Vector3 pos, Color color)[] Vertices;
        public readonly int[] Triangles;

        public MeshUpdatedEventArgs((Vector3 pos, Color color)[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }

        protected bool Equals(MeshUpdatedEventArgs other)
        {
            if (Vertices.Length != other.Vertices.Length) return false;
            foreach (var (first, second) in Vertices.Zip(other.Vertices, (arg1, arg2) => (arg1, arg2)))
            {
                if (!Equals(first, second)) return false;
            }

            if (Triangles.Length != other.Triangles.Length) return false;
            foreach (var (first, second) in Triangles.Zip(other.Triangles, (arg1, arg2) => (arg1, arg2)))
            {
                if (!Equals(first, second)) return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MeshUpdatedEventArgs)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Vertices != null ? Vertices.GetHashCode() : 0) * 397) ^
                        (Triangles != null ? Triangles.GetHashCode() : 0);
            }
        }
    }
}