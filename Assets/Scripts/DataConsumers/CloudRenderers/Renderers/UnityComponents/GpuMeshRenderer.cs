using System;
using System.Collections.Generic;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class GpuMeshRenderer : MonoBehaviour, IMeshRenderer, IGpuRenderer
    {
        [SerializeField] private Shader[] Shaders;

        public int ShaderId
        {
            get => _shaderId;
            set
            {
                if (_shaderId == value) return;
                _shaderId = value % Shaders.Length;
                lock (_blocks)
                {
                    foreach (var block in _blocks)
                    {
                        block.ShaderId = value;
                    }
                }
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;

                lock (_blocks)
                {
                    foreach (var block in _blocks)
                    {
                        block.Scale = value;
                    }
                }
            }
        }

        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            lock (_blocks)
            {
                while (e.Triangles.Length > _blocks.Count * MeshRendererBlock.Capacity)
                {
                    _blocks.Add(new MeshRendererBlock(Shaders));
                }

                var reservedSpace = _blocks.Count * MeshRendererBlock.Capacity;
                for (var i = 0; i < reservedSpace; i++)
                {
                    var blockIndex = i / MeshRendererBlock.Capacity;
                    var inBlockIndex = i % MeshRendererBlock.Capacity;

                    if (i < e.Triangles.Length)
                    {
                        _blocks[blockIndex][inBlockIndex] = new GPUItem(e.Vertices[e.Triangles[i]]);
                    }
                    else
                    {
                        _blocks[blockIndex][inBlockIndex] = default;
                    }
                }
            }
        }

        #region IQueueableRenderer

        public void UpdateDataOnGpu()
        {
            lock (_blocks)
            {
                foreach (var block in _blocks)
                {
                    block.UpdateDataOnGpu();
                }
            }
        }

        public void RenderNow()
        {
            lock (_blocks)
            {
                foreach (var block in _blocks)
                {
                    block.RenderData();
                }
            }
        }

        public int RenderQueue
        {
            get
            {
                var res = 0;
                lock (_blocks)
                {
                    if (_blocks.Count > 0) res = _blocks[0].RenderQueue;
                }

                return res;
            }
        }

        #endregion

        #region Unity events

        private void Update()
        {
            UpdateDataOnGpu();
        }
        
        #endregion

        #region Private

        private readonly List<MeshRendererBlock> _blocks = new List<MeshRendererBlock>();
        private float _scale;
        private int _shaderId;

        #endregion
    }
}