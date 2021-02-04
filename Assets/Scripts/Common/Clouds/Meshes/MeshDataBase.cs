using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    public abstract class MeshDataBase<T>
    {
        public ReaderWriterLockSlim Sync { get; private set; } = new ReaderWriterLockSlim();
        private int m_hasChanged;
        protected void MarkAsChanged() => Interlocked.CompareExchange(ref m_hasChanged, 1, 0);
        public bool HasChanged { get => Interlocked.Exchange(ref m_hasChanged, 0) == 1; }
        public int[] Indices { get; protected set; }
        public Vector3[] Vertices { get; protected set; }
        public Color[] Colors { get; protected set; }
        public Vector3[] Normals { get; protected set; }
        public abstract T Data { get; }
    }
}
