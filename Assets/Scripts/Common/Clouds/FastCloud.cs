using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public abstract class FastCloud<IMeshData, MeshObjectBase> : MonoBehaviour 
        where MeshObjectBase : MeshObjectBase<IMeshData>
    {
        public float scale = 1;
        public MeshObjectBase meshObjectPrefab;
        
        protected Dictionary<int, IMeshData> m_data;
        protected Dictionary<IMeshData, MeshObjectBase<IMeshData>> m_meshObjects;

        private ObjectPool m_meshObjectPool;
        private Queue<MeshDataBase<IMeshData>> m_newMeshQueue;
        private Queue<IMeshData> m_removedMeshQueue;

        protected virtual void Awake()
        {
            m_meshObjectPool = new ObjectPool(meshObjectPrefab.gameObject);
            m_data = new Dictionary<int, IMeshData>();
            m_meshObjects = new Dictionary<IMeshData, MeshObjectBase<IMeshData>>();
            m_newMeshQueue = new Queue<MeshDataBase<IMeshData>>();
            m_removedMeshQueue = new Queue<IMeshData>();
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
                    m_meshObjects[meshDataBase.Data] = m_meshObjectPool.Spawn().GetComponent<MeshObjectBase<IMeshData>>();
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
