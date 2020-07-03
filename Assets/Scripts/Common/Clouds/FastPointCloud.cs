using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public class FastPointCloud : MonoBehaviour, IFastPointsCloud
    {
        public float scale = 1;

        public PointsMeshObjectBase meshObjectPrefab;
        private ObjectPool m_meshObjectPool;
        private Dictionary<int, IPointsMeshData> m_data;
        private Dictionary<IPointsMeshData, MeshObjectBase<IPointsMeshData>> m_meshObjects;
        private Queue<MeshDataBase<IPointsMeshData>> m_newMeshQueue;
        private Queue<IPointsMeshData> m_removedMeshQueue;
        private int m_maxPointsCount;

        private void Awake()
        {
            m_meshObjectPool = new ObjectPool(meshObjectPrefab.gameObject);
            m_data = new Dictionary<int, IPointsMeshData>();
            m_meshObjects = new Dictionary<IPointsMeshData, MeshObjectBase<IPointsMeshData>>();
            m_newMeshQueue = new Queue<MeshDataBase<IPointsMeshData>>();
            m_removedMeshQueue = new Queue<IPointsMeshData>();
            m_maxPointsCount = meshObjectPrefab.MaxObjectsCount;
        }

        private void CheckMesh(int srcPointIdx, out int meshIdx, out int pointIdx)
        {
            meshIdx = srcPointIdx / m_maxPointsCount;
            if (!m_data.ContainsKey(meshIdx))
            {
                var data = meshObjectPrefab.CreateMeshData();
                m_data[meshIdx] = data.Data;
                lock(m_newMeshQueue) m_newMeshQueue.Enqueue(data);
            }
            pointIdx = srcPointIdx % m_maxPointsCount;
        }

        public void Clear()
        {
            foreach (var meshData in m_data)
            {
                lock(m_removedMeshQueue) m_removedMeshQueue.Enqueue(meshData.Value);
            }
            m_data.Clear();
        }

        private void Update()
        {
            lock (m_newMeshQueue)
            {
                while (m_newMeshQueue.Count != 0)
                {
                    var meshDataBase = m_newMeshQueue.Dequeue();
                    m_meshObjects[meshDataBase.Data] = m_meshObjectPool.Spawn().GetComponent<MeshObjectBase<IPointsMeshData>>();
                    m_meshObjects[meshDataBase.Data].Initialize(meshDataBase);
                }
            }
            lock (m_removedMeshQueue)
            {
                while (m_removedMeshQueue.Count != 0)
                {
                    var meshDataBase = m_removedMeshQueue.Dequeue();
                    m_meshObjectPool.Despawn(m_meshObjects[meshDataBase].gameObject);
                    m_meshObjects.Remove(meshDataBase);
                }
            }
        }

        public bool Exists(int idx)
        {
            int meshIdx = idx / m_maxPointsCount;
            if (m_data.ContainsKey(meshIdx))
            {
                int pointIdx = idx % m_maxPointsCount;
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

        public void Set(CloudPoint point)
        {
            CheckMesh(point.idx, out int meshId, out int pointId);
            m_data[meshId].Set(new CloudPoint(pointId, point.offset, point.color));
        }

        public void Set(IEnumerable<CloudPoint> points)
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

        public void Set(int idx, Matrix4x4 offset)
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
            var points = new CloudPoint[m_maxPointsCount * m_meshObjects.Count];
            KeyValuePair<int, IPointsMeshData>[] allMeshes = m_data.Select(kv => kv).ToArray();
            for (int meshNum = 0; meshNum < allMeshes.Length; ++meshNum)
            {
                var meshPoints = allMeshes[meshNum].Value.GetAll().ToArray();
                for (int i = 0; i < m_maxPointsCount; ++i)
                {
                    points[m_maxPointsCount * allMeshes[meshNum].Key + i] = new CloudPoint(
                        meshPoints[i].idx + meshNum * m_maxPointsCount,
                        meshPoints[i].offset,
                        meshPoints[i].color
                    );
                }
            }
            return points;
        }

        public void SetActive(bool value)
        {
            m_meshObjectPool.SetActive(value);
        }
    }
}
