using Elektronik.Common.Data.PackageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public partial class PointsMeshObject : PointsMeshObjectBase
    {
        public override int MaxObjectsCount => MeshData.MAX_VERTICES_COUNT;
        public override MeshDataBase<IPointsMeshData> CreateMeshData() => new MeshData();
        protected override MeshTopology MeshTopology => MeshTopology.Points;

        public class MeshData : MeshDataBase<IPointsMeshData>, IPointsMeshData
        {
            public const int MAX_VERTICES_COUNT = 65000;
            public const int MAX_POINTS_COUNT = MAX_VERTICES_COUNT;

            public MeshData()
            {
                Indices = Enumerable.Range(0, MAX_VERTICES_COUNT).ToArray();
                Vertices = new Vector3[MAX_VERTICES_COUNT];
                Colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            }

            public CloudPoint Get(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
#endif
                CloudPoint result;
                try
                {
                    Sync.EnterReadLock();
                    result = new CloudPoint(idx, Vertices[idx], Colors[idx]);
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return result;
            }

            public void Set(IEnumerable<CloudPoint> points)
            {
                //var points_ = points.ToArray();
                try
                {
                    Sync.EnterWriteLock();
                    //Parallel.For(0, points_.Length, i =>
                    foreach(var pt in points)
                    {
                        Colors[pt.idx] = pt.color;
                        Vertices[pt.idx] = pt.offset;
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
#if DEBUG
                Debug.Assert(point.idx >= 0 && point.idx < MAX_VERTICES_COUNT, $"Wrong idx ({point.idx})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    Colors[point.idx] = point.color;
                    Vertices[point.idx] = point.offset;
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
                Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    Colors[idx] = color;
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Vector3 position)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    Vertices[idx] = position;
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public IEnumerable<CloudPoint> GetAll()
            {
                var points = new List<CloudPoint>(MAX_POINTS_COUNT);
                try
                {
                    Sync.EnterReadLock();
                    for (int i = 0; i < MAX_POINTS_COUNT; ++i)
                    {
                        if (Exists(i))
                        {
                            points.Add(Get(i));
                        }
                    }
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return points;
            }

            public void Clear()
            {
                try
                {
                    Sync.EnterWriteLock();
                    for (int i = 0; i < Vertices.Length; ++i)
                    {
                        Vertices[i] = Vector3.zero;
                        Colors[i] = new Color(0, 0, 0, 0);
                    }
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
            }

            public bool Exists(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_VERTICES_COUNT, $"Wrong idx ({idx})");
#endif
                bool res = false;
                try
                {
                    Sync.EnterReadLock();
                    res = !(Vertices[idx] == Vector3.zero && Colors[idx] == new Color(0, 0, 0, 0));
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return res;
            }

            public override IPointsMeshData Data { get => this; }
        }
    }
}
