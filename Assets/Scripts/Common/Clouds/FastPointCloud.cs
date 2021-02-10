using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPointCloud : FastCloud<IPointsMeshData, PointsMeshObjectBase>, IFastPointsCloud
    {
        private int MaxPointsCount { get => meshObjectPrefab.MaxObjectsCount; }

        public bool Exists(int idx)
        {
            int meshIdx = idx / MaxPointsCount;
            if (m_data.ContainsKey(meshIdx))
            {
                int pointIdx = idx % MaxPointsCount;
                return m_data[meshIdx].Exists(pointIdx);
            }
            else
            {
                return false;
            }
        }

        public CloudPoint Get(int idx)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            return m_data[meshId].Get(pointId);
        }

        public void Add(IEnumerable<CloudPoint> points)
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePoint(CloudPoint point)
        {
            CheckMesh(point.idx, out int meshId, out int pointId);
            m_data[meshId].Set(new CloudPoint(pointId, point.offset, point.color));
        }

        public void UpdatePoints(IEnumerable<CloudPoint> points)
        {
            var packets = new Dictionary<int, List<CloudPoint>>();
            foreach (var pt in points)
            {
                CheckMesh(pt.idx, out var meshIdx, out var pointIdx);
                if (!packets.ContainsKey(meshIdx))
                {
                    packets[meshIdx] = new List<CloudPoint>();
                }
                packets[meshIdx].Add(new CloudPoint(pointIdx, pt.offset, pt.color));
            }
            foreach (var packet in packets)
            {
                m_data[packet.Key].Set(packet.Value);
            }
        }

        public void Remove(int idx)
        {
            Set(idx, new Color(0, 0, 0, 0));
        }

        public void Remove(IEnumerable<int> pointsIds)
        {
            foreach (var id in pointsIds)
            {
                Remove(id);
            }
        }

        public void Set(int idx, Vector3 offset)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_data[meshId].Set(pointId, offset);
        }

        public void Set(int idx, Color color)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_data[meshId].Set(pointId, color);
        }

        public IEnumerable<CloudPoint> GetAll()
        {
            var points = new CloudPoint[MaxPointsCount * m_meshObjects.Count];
            KeyValuePair<int, IPointsMeshData>[] allMeshes = m_data.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                var meshPoints = allMeshes[meshNum].Value.GetAll().ToArray();
                for (int i = 0; i < MaxPointsCount; ++i)
                {
                    points[MaxPointsCount * allMeshes[meshNum].Key + i] = new CloudPoint(
                        meshPoints[i].idx + meshNum * MaxPointsCount,
                        meshPoints[i].offset,
                        meshPoints[i].color
                    );
                }
            }
            return points;
        }

        public void Add(CloudPoint point)
        {
            throw new System.NotImplementedException();
        }
    }
}
