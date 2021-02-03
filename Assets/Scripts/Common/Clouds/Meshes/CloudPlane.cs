using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    public class CloudPlane
    {
        public static CloudPlane Empty(int id) => new CloudPlane(id, Vector3.zero, Vector3.up, new Color(0, 0, 0, 0));
        public int idx;
        public Vector3 offset;
        public Vector3 normal;
        public Color color;
        public CloudPlane(int idx, Vector3 offset, Vector3 normal, Color color)
        {
            this.idx = idx;
            this.offset = offset;
            this.color = color;
            this.normal = normal;
        }
    }
}