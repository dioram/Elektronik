using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PlanesMeshObject : PlanesMeshObjectBase
    {
        public float SideSize = 2.0f;
        
        public override int MaxObjectsCount => MeshData.MAX_PLANES_COUNT;
        public override MeshDataBase<IPlanesMeshData> CreateMeshData() => new MeshData(SideSize);

        protected override MeshTopology MeshTopology => MeshTopology.Triangles; 
        
        public class MeshData : MeshDataBase<IPlanesMeshData>, IPlanesMeshData
        {
            public const int INDICES_PER_PLANE = 8;
            public const int MAX_VERTICES_COUNT = 64992;
            public const int MAX_PLANES_COUNT = MAX_VERTICES_COUNT / INDICES_PER_PLANE;

            public static readonly Color DEFAULT_COLOR = Color.red;

            private float m_sideSize;
            
            
            public MeshData(float sideSize)
            {
                m_sideSize = sideSize;
                var plane = new []
                {
                    0, 1, 3,
                    1, 2, 3,
                    7, 5, 4,
                    7, 6, 5,
                };
                Indices = Enumerable
                    .Range(0, MAX_PLANES_COUNT)
                    .SelectMany(idx => plane.Select(i => i + idx * INDICES_PER_PLANE))
                    .ToArray();
                Vertices = new Vector3[MAX_VERTICES_COUNT];
                Normals = new Vector3[MAX_VERTICES_COUNT];
                Colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            }

            public override IPlanesMeshData Data => this;
            
            
            public CloudPlane Get(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_PLANES_COUNT, $"Wrong idx ({idx})");
#endif
                CloudPlane plane;
                var planeOffset = Vertices[idx * INDICES_PER_PLANE];
                var planeNormal = Vector3.Cross(Vertices[idx * INDICES_PER_PLANE + 1], Vertices[idx * INDICES_PER_PLANE + 2]);
                plane = new CloudPlane(idx, planeOffset, planeNormal, Colors[idx * INDICES_PER_PLANE]);
                return plane;
            }

            private void PureSet(int idx, Color color)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_PLANES_COUNT, $"Wrong idx ({idx})");
#endif
                for (int i = 0; i < INDICES_PER_PLANE; ++i)
                {
                    Colors[idx * INDICES_PER_PLANE + i] = color;
                }
            }

            private void PureSet(int idx, Vector3 offset, Vector3 normal)
            {
                if (Colors[idx * INDICES_PER_PLANE] == new Color(0, 0, 0, 0))
                {
                    PureSet(idx, DEFAULT_COLOR);
                }
                
                float halfSide = m_sideSize / 2;
                var v1 = new Vector3(-halfSide, 0, -halfSide);
                var v2 = new Vector3(halfSide, 0, -halfSide);
                var v3 = new Vector3(halfSide, 0, halfSide);
                var v4 = new Vector3(-halfSide, 0, halfSide);

                var rotation = Quaternion.FromToRotation(Vector3.up, normal);
                
                Vertices[idx * INDICES_PER_PLANE + 0] = rotation * v1 + offset;
                Vertices[idx * INDICES_PER_PLANE + 1] = rotation * v2 + offset;
                Vertices[idx * INDICES_PER_PLANE + 2] = rotation * v3 + offset;
                Vertices[idx * INDICES_PER_PLANE + 3] = rotation * v4 + offset;
                Vertices[idx * INDICES_PER_PLANE + 4] = rotation * v1 + offset;
                Vertices[idx * INDICES_PER_PLANE + 5] = rotation * v2 + offset;
                Vertices[idx * INDICES_PER_PLANE + 6] = rotation * v3 + offset;
                Vertices[idx * INDICES_PER_PLANE + 7] = rotation * v4 + offset;

                for (int i = 0; i < 4; i++)
                {
                    Normals[idx * INDICES_PER_PLANE + i] = normal;
                }
                for (int i = 0; i < 4; i++)
                {
                    Normals[idx * INDICES_PER_PLANE + i + 4] = normal;
                }
            }

            public void Set(CloudPlane plane)
            {
                PureSet(plane.idx, plane.offset, plane.normal);
                PureSet(plane.idx, plane.color);
                MarkAsChanged();
            }
            

            public void Set(IEnumerable<CloudPlane> planes)
            {
                foreach(var plane in planes)
                {
                    PureSet(plane.idx, plane.offset, plane.normal);
                    PureSet(plane.idx, plane.color);
                }
                MarkAsChanged();
            }

            public void Set(int idx, Color color)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_PLANES_COUNT, $"Wrong idx ({idx})");
#endif
                PureSet(idx, color);
                MarkAsChanged();
            }

            public void Set(int idx, Vector3 offset, Vector3 normal)
            {
                PureSet(idx, offset, normal);
                MarkAsChanged();
            }

            public bool Exists(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_PLANES_COUNT, $"Wrong idx ({idx})");
#endif
                bool notExists = true;
                for (int i = 0; i < INDICES_PER_PLANE; ++i)
                {
                    notExists = notExists && (Colors[idx * INDICES_PER_PLANE + i] == new Color(0, 0, 0, 0));
                }
                return !notExists;
            }

            public IEnumerable<CloudPlane> GetAll()
            {
                var planes = new List<CloudPlane>(MAX_PLANES_COUNT);
                for (int i = 0; i < MAX_PLANES_COUNT; ++i)
                {
                    if (Exists(i))
                    {
                        planes.Add(Get(i));
                    }
                }
                return planes;
            }

            public void Clear()
            {
                try
                {
                    Sync.EnterWriteLock();
                    Parallel.For(0, Vertices.Length, i =>
                    {
                        Vertices[i] = Vector3.zero;
                        Colors[i] = new Color(0, 0, 0, 0);
                    });
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }
        }
    }
}