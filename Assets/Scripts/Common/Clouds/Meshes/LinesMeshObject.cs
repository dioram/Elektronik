using System;
using System.Collections.Generic;
using System.ComponentModel;
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
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
#endif
                CloudLine line = Get(idx);
                bool res = false;
                try
                {
                    Sync.EnterReadLock();
                    res = 
                        (line.pt1.offset != Vector3.zero) ||
                        (line.pt2.offset != Vector3.zero) ||
                        (line.pt1.color != new Color(0, 0, 0, 0)) || 
                        (line.pt2.color != new Color(0, 0, 0, 0));
                }
                finally
                {
                    Sync.ExitReadLock();
                }
                return res;
            }

            public CloudLine Get(int idx)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
#endif
                try
                {
                    Sync.EnterReadLock();
                    int vertex1Idx = 2 * idx;
                    int vertex2Idx = vertex1Idx + 1;
                    return new CloudLine(idx, 
                        new CloudPoint(vertex1Idx, Vertices[vertex1Idx], Colors[vertex1Idx]),
                        new CloudPoint(vertex2Idx, Vertices[vertex2Idx], Colors[vertex2Idx]));
                }
                finally
                {
                    Sync.ExitReadLock();
                }
            }

            private void PureSet(CloudLine line)
            {
                Colors[2 * line.Id] = line.pt1.color;
                Colors[2 * line.Id + 1] = line.pt2.color;
                Vertices[2 * line.Id] = line.pt1.offset;
                Vertices[2 * line.Id + 1] = line.pt2.offset;
            }

            public void Set(IEnumerable<CloudLine> lines)
            {
                try
                {
                    Sync.EnterWriteLock();
                    foreach (var line in lines)
                    {
                        PureSet(line);
                    }
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(CloudLine line)
            {
#if DEBUG
                Debug.Assert(line.Id >= 0 && line.Id < MAX_LINES_COUNT, $"Wrong idx ({line.Id})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    PureSet(line);
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Color color1, Color color2)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    int index1 = 2 * idx;
                    int index2 = 2 * idx + 1;
                    PureSet(new CloudLine(
                        idx, 
                        new CloudPoint(index1, Vertices[index1], color1),
                        new CloudPoint(index2, Vertices[index2], color2)));
                }
                finally
                {
                    Sync.ExitWriteLock();
                }
                MarkAsChanged();
            }

            public void Set(int idx, Vector3 position1, Vector3 position2)
            {
#if DEBUG
                Debug.Assert(idx >= 0 && idx < MAX_LINES_COUNT, $"Wrong idx ({idx})");
#endif
                try
                {
                    Sync.EnterWriteLock();
                    int index1 = 2 * idx;
                    int index2 = 2 * idx + 1;
                    PureSet(new CloudLine(
                        idx,
                        new CloudPoint(index1, position1, Colors[index1]),
                        new CloudPoint(index2, position2, Colors[index2])));
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
