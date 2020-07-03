using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class LinesMeshObject : LinesMeshObjectBase
    {
        public override int MaxObjectsCount => MeshData.MAX_LINES_COUNT;
        public override MeshDataBase<ILinesMeshData> CreateMeshData() => new MeshData();
        protected override MeshTopology MeshTopology => MeshTopology.Lines;
        public class MeshData : MeshDataBase<ILinesMeshData>, ILinesMeshData
        {
            public const int MAX_VERTICES_COUNT = 65000;
            public const int MAX_LINES_COUNT = MAX_VERTICES_COUNT / 2;

            public int Max => MAX_LINES_COUNT;

            public MeshData()
            {
                Indices = Enumerable.Range(0, MAX_VERTICES_COUNT).ToArray();
                Vertices = new Vector3[MAX_VERTICES_COUNT];
                Colors = Enumerable.Repeat(new Color(0, 0, 0, 0), MAX_VERTICES_COUNT).ToArray();
            }

            public bool Exists(int idx)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                Get(idx, out Vector3 pos1, out Vector3 pos2, out Color color);
                bool res = false;
                try
                {
                    Sync.EnterReadLock();
                    res = !(pos1 == Vector3.zero && pos2 == Vector3.zero && color == new Color(0, 0, 0, 0));
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return res;
            }

            public void Get(int idx, out Vector3 position1, out Vector3 position2, out Color color)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                try
                {
                    Sync.EnterReadLock();
                    position1 = Vertices[2 * idx];
                    position2 = Vertices[2 * idx + 1];
                    color = Colors[2 * idx];
                }
                finally
                {
                    Sync.ExitReadLock();
                }
            }

            public void Set(int idx, Vector3 position1, Vector3 position2, Color color1, Color color2)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                Set(idx, color1, color2);
                Set(idx, position1, position2);
                MarkAsChanged();
            }

            public void Set(int idx, Vector3 position1, Vector3 position2, Color color)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                Set(idx, color);
                Set(idx, position1, position2);
                MarkAsChanged();
            }

            public void Set(int idx, Color color1, Color color2)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                try
                {
                    Sync.EnterWriteLock();
                    Colors[2 * idx] = color1;
                    Colors[2 * idx + 1] = color2;
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Color color)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                try
                {
                    Sync.EnterWriteLock();
                    Colors[2 * idx] = color;
                    Colors[2 * idx + 1] = color;
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Vector3 position1, Vector3 position2)
            {
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
                try
                {
                    Sync.EnterWriteLock();
                    Vertices[2 * idx] = position1;
                    Vertices[2 * idx + 1] = position2;
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Clear()
            {
                try
                {
                    Sync.EnterWriteLock();
                    Parallel.For(0, MAX_VERTICES_COUNT, i =>
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

            public override ILinesMeshData Data { get => this; }
        }
    }
}
