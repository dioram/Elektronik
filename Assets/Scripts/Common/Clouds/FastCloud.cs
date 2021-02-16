using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public abstract class FastCloud<TMeshData, TMeshObjectBase> : MonoBehaviour 
        where TMeshObjectBase : MeshObjectBase<TMeshData>
    {
        public float scale = 1;
        public TMeshObjectBase meshObjectPrefab;
        
        protected Dictionary<int, TMeshData> m_data;
        protected Dictionary<TMeshData, MeshObjectBase<TMeshData>> m_meshObjects;

        private ObjectPool m_meshObjectPool;
        private Queue<MeshDataBase<TMeshData>> m_newMeshQueue;
        private Queue<TMeshData> m_removedMeshQueue;

        protected virtual void Awake()
        {
            m_meshObjectPool = new ObjectPool(meshObjectPrefab.gameObject);
            m_data = new Dictionary<int, TMeshData>();
            m_meshObjects = new Dictionary<TMeshData, MeshObjectBase<TMeshData>>();
            m_newMeshQueue = new Queue<MeshDataBase<TMeshData>>();
            m_removedMeshQueue = new Queue<TMeshData>();
        }

        protected void CheckMesh(int srcLineIdx, out int meshIdx, out int lineIdx)
        {
            meshIdx = srcLineIdx / meshObjectPrefab.MaxObjectsCount;
            if (!m_data.ContainsKey(meshIdx))
            {
                var mesh = meshObjectPrefab.CreateMeshData();
                m_data[meshIdx] = mesh.Data;
                lock (m_newMeshQueue) m_newMeshQueue.Enqueue(mesh);
            }
            lineIdx = srcLineIdx % meshObjectPrefab.MaxObjectsCount;
        }

        private void OnEnable()
        {
            // clear the cloud if activation is invoked after despawning
            ForcedClear();

            foreach (var meshData in m_data)
            {
                m_meshObjects[meshData.Value].gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var meshData in m_data)
            {
                if (m_meshObjects[meshData.Value] != null)
                    m_meshObjects[meshData.Value].gameObject.SetActive(false);
            }
        }

        public void Clear()
        {
            foreach (var meshData in m_data)
            {
                lock (m_removedMeshQueue) m_removedMeshQueue.Enqueue(meshData.Value);
            }
            m_data.Clear();
        }

        private void ForcedSpawn()
        {
            lock (m_newMeshQueue)
            {
                while (m_newMeshQueue.Count != 0)
                {
                    var meshDataBase = m_newMeshQueue.Dequeue();
                    m_meshObjects[meshDataBase.Data] = m_meshObjectPool.Spawn().GetComponent<MeshObjectBase<TMeshData>>();
                    m_meshObjects[meshDataBase.Data].Initialize(meshDataBase);
                }
            }
        }

        private void ForcedClear()
        {
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

        private void ForcedUpdate()
        {
            ForcedSpawn();
            ForcedClear();
        }

        private void Update()
        {
            ForcedUpdate();
        }

        public void SetActive(bool value)
        {
            m_meshObjectPool.SetActive(value);
        }
    }

}
