using System;
using System.Collections.Generic;
using System.Threading;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class GpuMeshRenderer : MonoBehaviour, IMeshRenderer
    {
        [SerializeField] private Shader[] Shaders;

        public int ShaderId
        {
            get => _shaderId;
            set
            {
                if (_shaderId == value) return;
                _shaderId = value % Shaders.Length;
                foreach (var block in _blocks)
                {
                    block.ShaderId = value;
                }
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;
                
                foreach (var block in _blocks)
                {
                    block.SetScale(value);
                } 
            }
        }

        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            if (CheckAndCreateReserves(e.Vertices, e.Triangles)) return;
            UpdateMesh(e.Vertices, e.Triangles);
        }

        #region Private

        private readonly List<MeshRendererBlock> _blocks = new List<MeshRendererBlock>();
        private float _scale;
        private int _shaderId;

        private void UpdateMesh((Vector3 pos, Color color)[] vertices, int[] indexes, bool takeLock = true)
        {
            if (takeLock) Monitor.Enter(_blocks);

            var reservedSpace = _blocks.Count * MeshRendererBlock.Capacity;
            for (var i = 0; i < reservedSpace; i++)
            {
                var blockIndex = i / MeshRendererBlock.Capacity;
                var inBlockIndex = i % MeshRendererBlock.Capacity;

                if (i < indexes.Length)
                {
                    _blocks[blockIndex].Vertices[inBlockIndex] = new GPUItem(vertices[indexes[i]]);
                }
                else
                {
                    _blocks[blockIndex].Vertices[inBlockIndex] = default;
                }
            }

            foreach (var block in _blocks)
            {
                block.Updated = true;
            }

            if (takeLock) Monitor.Exit(_blocks);
        }
        
        private bool CheckAndCreateReserves((Vector3 pos, Color color)[] vertices, int[] indexes)
        {
            if (indexes.Length > _blocks.Count * MeshRendererBlock.Capacity)
            {
                // reserves overloaded
                MainThreadInvoker.Enqueue(() =>
                {
                    lock (_blocks)
                    {
                        var requiredSpace = (indexes.Length - _blocks.Count * MeshRendererBlock.Capacity) +
                                MeshRendererBlock.Capacity;
                        var requiredBlocks = requiredSpace / MeshRendererBlock.Capacity + 1;
                        for (var i = 0; i < requiredBlocks; i++)
                        {
                            CreateNewBlock();
                        }

                        UpdateMesh(vertices, indexes, false);
                    }
                });
                return true;
            }

            if (indexes.Length > (_blocks.Count - 1) * MeshRendererBlock.Capacity)
            {
                // add reserves
                MainThreadInvoker.Enqueue(CreateNewBlock);
            }

            return false;
        }

        private void CreateNewBlock()
        {
            var go = new GameObject($"{GetType().Name} {_blocks.Count}");
            try
            {
                go.transform.SetParent(transform);
            }
            catch (NullReferenceException)
            {
                // Sometimes for unknown reasons this.transform causes NullReferenceException.
                // We can just live with that.
            }
            var block = go.AddComponent<MeshRendererBlock>();
            block.Shaders = Shaders;
            block.Updated = true;
            _blocks.Add(block);
        }

        #endregion
    }
}