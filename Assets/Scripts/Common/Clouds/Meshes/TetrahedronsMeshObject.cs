using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class TetrahedronsMeshObject : PointsMeshObjectBase
    {
        public float sideSize = .1f;
        public bool needOrientation = false;

        public override int MaxObjectsCount => MeshData.MAX_THETRAHEDRONS_COUNT;
        public override MeshDataBase<IPointsMeshData> CreateMeshData() => new MeshData(sideSize, needOrientation);
        protected override MeshTopology MeshTopology => MeshTopology.Triangles;

        public class MeshData : MeshDataBase<IPointsMeshData>, IPointsMeshData
        {
            public const int INDICES_PER_THETRAHEDRON = 4;
            public const int MAX_VERTICES_COUNT = 64992;
            public const int MAX_THETRAHEDRONS_COUNT = MAX_VERTICES_COUNT / INDICES_PER_THETRAHEDRON;

            private float m_sideSize;
            private bool m_needOrientation;

            public MeshData(float sideSize, bool needOrientation)
            {
                m_sideSize = sideSize;
                m_needOrientation = needOrientation;
                var thetrahedron = new []
                {
                    3, 2, 1,
                    1, 2, 0,
                    2, 3, 0,
                    0, 3, 1,
                };
                Indices = Enumerable
                    .Range(0, MAX_THETRAHEDRONS_COUNT)
                    .SelectMany(idx => thetrahedron.Select(i => i + idx * INDICES_PER_THETRAHEDRON))
                    .ToArray();
                Vertices = new Vector3[MAX_VERTICES_COUNT];
                Colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            }

            public bool Exists(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, $"Wrong idx ({idx})");
#endif
                bool notExists = true;
                try
                {
                    Sync.EnterReadLock();
                    for (int i = 0; i < INDICES_PER_THETRAHEDRON; ++i)
                    {
                        notExists = notExists && (Colors[idx * INDICES_PER_THETRAHEDRON + i] == new Color(0, 0, 0, 0));
                    }
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return !notExists;
            }

            public CloudPoint Get(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, $"Wrong idx ({idx})");
#endif
                var tetrahedronCG = new Vector3();
                CloudPoint point;
                try
                {
                    Sync.EnterReadLock();
                    for (int i = 0; i < INDICES_PER_THETRAHEDRON; ++i)
                    {
                        tetrahedronCG += Vertices[idx * INDICES_PER_THETRAHEDRON + i];
                    }
                    tetrahedronCG /= INDICES_PER_THETRAHEDRON;
                    point = new CloudPoint(idx, tetrahedronCG, Colors[idx * INDICES_PER_THETRAHEDRON]);
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return point;
            }

            private void PureSet(int idx, Color color)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, $"Wrong idx ({idx})");
#endif
                int init = m_needOrientation ? 3 : 0;
                for (int i = init; i < INDICES_PER_THETRAHEDRON; ++i)
                {
                    Colors[idx * INDICES_PER_THETRAHEDRON + i] = color;
                }
            }

            private void PureSet(int idx, Vector3 offset)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, $"Wrong idx ({idx})");
#endif
                float height = 0.86603f * m_sideSize;
                float halfHeight = height / 2;
                float halfSide = m_sideSize / 2f;
                float x = Mathf.Sqrt(m_sideSize * m_sideSize - height * height);
                float triangleHeight = Mathf.Sqrt(3f / 4) * m_sideSize;
                             
                var v0 = new Vector3(0, halfHeight, 0);
                var v1 = new Vector3(-x, -halfHeight, 0);
                var v2 = new Vector3(triangleHeight - x, -halfHeight, halfSide);
                var v3 = new Vector3(triangleHeight - x, -halfHeight, -halfSide);

                Vertices[idx * INDICES_PER_THETRAHEDRON + 0] = v0 + offset;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 1] = v1 + offset;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 2] = v2 + offset;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 3] = v3 + offset;
            }

            public void Set(IEnumerable<CloudPoint> points)
            {
                try
                {
                    Sync.EnterWriteLock();
                    foreach(var pt in points)
                    {
                        PureSet(pt.idx, pt.offset);
                        PureSet(pt.idx, pt.color);
                    }
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(CloudPoint point)
            {
                try
                {
                    Sync.EnterWriteLock();
                    PureSet(point.idx, point.offset);
                    PureSet(point.idx, point.color);
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Color color)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_THETRAHEDRONS_COUNT, $"Wrong idx ({idx})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    PureSet(idx, color);
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Vector3 offset)
            {
                try
                {
                    Sync.EnterWriteLock();
                    PureSet(idx, offset);
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public IEnumerable<CloudPoint> GetAll()
            {
                var points = new List<CloudPoint>(MAX_THETRAHEDRONS_COUNT);
                for (int i = 0; i < MAX_THETRAHEDRONS_COUNT; ++i)
                {
                    if (Exists(i))
                    {
                        points.Add(Get(i));
                    }
                }
                return points;
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

            public override IPointsMeshData Data { get => this; }
        }
    }
}
