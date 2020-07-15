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
            public const int INDICES_PER_THETRAHEDRON = 12;
            public const int MAX_VERTICES_COUNT = 64992;
            public const int MAX_THETRAHEDRONS_COUNT = MAX_VERTICES_COUNT / INDICES_PER_THETRAHEDRON;

            private float m_sideSize;
            private bool m_needOrientation;

            public MeshData(float sideSize, bool needOrientation)
            {
                m_sideSize = sideSize;
                m_needOrientation = needOrientation;
                Indices = Enumerable.Range(0, MAX_VERTICES_COUNT).ToArray();
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
                    for (int i = 0; i < MAX_VERTICES_COUNT; ++i)
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
                float halfHeight = 0.86603f * m_sideSize / 2;
                float halfSide = m_sideSize / 2f;
                Vector3 toCenter = new Vector3(-halfSide, -halfHeight, -halfHeight);
                Vector3 v0 = new Vector3(0, 0, 0.86603f * m_sideSize) + toCenter;
                Vector3 v1 = new Vector3(halfSide, 0.86603f * m_sideSize, 0.86603f * m_sideSize) + toCenter;
                Vector3 v2 = new Vector3(m_sideSize, 0, 0.86603f * m_sideSize) + toCenter;
                Vector3 v3 = new Vector3(halfSide, halfHeight, 0) + toCenter;

                Vertices[idx * INDICES_PER_THETRAHEDRON + 0] = v2;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 1] = v1;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 2] = v0;

                Vertices[idx * INDICES_PER_THETRAHEDRON + 3] = v3;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 4] = v2;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 5] = v0;

                Vertices[idx * INDICES_PER_THETRAHEDRON + 6] = v3;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 7] = v1;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 8] = v2;

                Vertices[idx * INDICES_PER_THETRAHEDRON + 9] = v1;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 10] = v3;
                Vertices[idx * INDICES_PER_THETRAHEDRON + 11] = v0;

                for (int i = 0; i < INDICES_PER_THETRAHEDRON; ++i)
                {
                    var t = Vertices[idx * INDICES_PER_THETRAHEDRON + i];
                    Vertices[idx * INDICES_PER_THETRAHEDRON + i] = t + offset;
                }
            }

            public void Set(IEnumerable<CloudPoint> points)
            {
                // var points_ = points.ToArray();
                try
                {
                    Sync.EnterWriteLock();
                    //Parallel.For(0, points_.Length, i =>
                    foreach(var pt in points)
                    {
                        PureSet(pt.idx, pt.offset);
                        PureSet(pt.idx, pt.color);
                    }//);
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
                int init = m_needOrientation ? 3 : 0;
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
