using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds.Meshes;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPlaneCloud : FastCloud<IPlanesMeshData, PlanesMeshObjectBase>, IFastPlanesCloud
    {
        private int MaxPlanesCount => meshObjectPrefab.MaxObjectsCount;

        public bool Exists(int idx)
        {
            int meshIdx = idx / MaxPlanesCount;
            if (!m_data.ContainsKey(meshIdx)) return false;

            int pointIdx = idx % MaxPlanesCount;
            return m_data[meshIdx].Exists(pointIdx);
        }

        public CloudPlane Get(int idx)
        {
            CheckMesh(idx, out int meshId, out int planeId);
            return m_data[meshId].Get(planeId);
        }

        public IEnumerable<CloudPlane> GetAll()
        {
            var planes = new CloudPlane[MaxPlanesCount * m_meshObjects.Count];
            KeyValuePair<int, IPlanesMeshData>[] allMeshes = m_data.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                var meshPlanes = allMeshes[meshNum].Value.GetAll().ToArray();
                for (int i = 0; i < MaxPlanesCount; ++i)
                {
                    planes[MaxPlanesCount * allMeshes[meshNum].Key + i] = new CloudPlane(
                        meshPlanes[i].idx + meshNum * MaxPlanesCount,
                        meshPlanes[i].offset,
                        meshPlanes[i].normal,
                        meshPlanes[i].color
                    );
                }
            }

            return planes;
        }

        public void Set(CloudPlane plane)
        {
            CheckMesh(plane.idx, out int meshId, out int pointId);
            m_data[meshId].Set(new CloudPlane(pointId, plane.offset, plane.normal, plane.color));
        }

        public void Set(int idx, Color color)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_data[meshId].Set(pointId, color);
        }

        public void Set(int idx, Vector3 offset, Vector3 normal)
        {
            CheckMesh(idx, out int meshId, out int pointId);
            m_data[meshId].Set(pointId, offset, normal);
        }

        public void Set(IEnumerable<CloudPlane> planes)
        {
            var packets = new Dictionary<int, List<CloudPlane>>();
            foreach (var plane in planes)
            {
                CheckMesh(plane.idx, out var meshIdx, out var pointIdx);
                if (!packets.ContainsKey(meshIdx))
                {
                    packets[meshIdx] = new List<CloudPlane>();
                }

                packets[meshIdx].Add(new CloudPlane(pointIdx, plane.offset, plane.normal, plane.color));
            }

            foreach (var packet in packets)
            {
                m_data[packet.Key].Set(packet.Value);
            }
        }
    }
}