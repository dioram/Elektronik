using System;
using System.Collections.Generic;
using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for meshes. </summary>
    internal class GpuMeshRenderer : IMeshRenderer, IGpuRenderer
    {
        
        /// <summary> Constructor. </summary>
        /// <param name="shaders"> Shaders for rendering meshes. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public GpuMeshRenderer(Shader[] shaders, float scale)
        {
            _shaders = shaders;
            _scale = scale;
        }

        #region IMeshRenderer

        /// <inheritdoc />
        public int ShaderId
        {
            get => _shaderId;
            set
            {
                if (_shaderId == value) return;
                _shaderId = value % _shaders.Length;
                lock (_blocks)
                {
                    foreach (var block in _blocks)
                    {
                        block.ShaderId = value;
                    }
                }
            }
        }

        /// <inheritdoc cref="IScalable" />
        public float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;

                _scale = value;
                lock (_blocks)
                {
                    foreach (var block in _blocks)
                    {
                        block.Scale = value;
                    }
                }
            }
        }
        
        /// <inheritdoc />
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            // TODO: Add support for multiple senders.
            lock (_blocks)
            {
                while (e.Triangles.Length > _blocks.Count * MeshRendererBlock.Capacity)
                {
                    _blocks.Add(new MeshRendererBlock(_shaders, Scale));
                }

                var reservedSpace = _blocks.Count * MeshRendererBlock.Capacity;
                for (var i = 0; i < reservedSpace; i++)
                {
                    var blockIndex = i / MeshRendererBlock.Capacity;
                    var inBlockIndex = i % MeshRendererBlock.Capacity;

                    if (i < e.Triangles.Length)
                    {
                        var (vert, color) = e.Vertices[e.Triangles[i]];
                        _blocks[blockIndex][inBlockIndex] = new GpuItem(vert, color);
                    }
                    else
                    {
                        _blocks[blockIndex][inBlockIndex] = default;
                    }
                }
            }
        }
        
        public void Dispose()
        {
            foreach (var block in _blocks)
            {
                block.Dispose();
            }
            _blocks.Clear();
        }

        #endregion

        #region IGpuRenderer

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        #region Private

        private readonly Shader[] _shaders;
        private readonly List<MeshRendererBlock> _blocks = new List<MeshRendererBlock>();
        private float _scale;
        private int _shaderId;

        #endregion
    }
}