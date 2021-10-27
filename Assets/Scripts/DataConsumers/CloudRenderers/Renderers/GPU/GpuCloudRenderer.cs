using System;
using System.Collections.Generic;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base class for rendering object clouds. Such as points, lines, etc. </summary>
    /// <typeparam name="TCloudItem"> Type of items it can render. </typeparam>
    /// <typeparam name="TCloudBlock"> Type of blocks this renderer is using. </typeparam>
    /// <typeparam name="TGpuItem"> Type of data that will be sent on GPU. </typeparam>
    internal abstract class GpuCloudRenderer<TCloudItem, TCloudBlock, TGpuItem>
            : ICloudRenderer<TCloudItem>, IGpuRenderer
            where TCloudItem : struct, ICloudItem
            where TCloudBlock : class, ICloudBlock<TGpuItem>
    {
        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public GpuCloudRenderer(Shader shader, float scale)
        {
            Shader = shader;
            _scale = scale;
        }

        /// <summary> Read-only getter for blocks. </summary>
        public IReadOnlyCollection<TCloudBlock> Blocks => _blocks.AsReadOnly();

        #region IGpuRenderer

        /// <inheritdoc />
        public void UpdateDataOnGpu()
        {
            lock (_pointPlaces)
            {
                foreach (var block in Blocks)
                {
                    block.UpdateDataOnGpu();
                }
            }
        }

        /// <summary>
        /// Renders data on GPU. Should be called in MainThread and in MonoBehaviour.OnRenderObject().
        /// </summary>
        public void RenderNow()
        {
            lock (_pointPlaces)
            {
                foreach (var block in Blocks)
                {
                    block.RenderData();
                }
            }
        }

        /// <inheritdoc />
        public int RenderQueue => _blocks.Count > 0 ? _blocks[0].RenderQueue : 0;

        #endregion

        #region ICloudRenderer

        /// <inheritdoc />
        public int ItemsCount
        {
            get
            {
                lock (_pointPlaces)
                {
                    return _pointPlaces.Count;
                }
            }
        }

        /// <inheritdoc />
        public float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;

                _scale = value;
                foreach (var block in _blocks)
                {
                    block.Scale = value;
                }
            }
        }

        /// <inheritdoc />
        public void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            var newAmountOfItems = _amountOfItems + e.AddedItems.Count;
            lock (_pointPlaces)
            {
                foreach (var item in e.AddedItems)
                {
                    var index = _freePlaces.Count > 0 ? _freePlaces.Dequeue() : _maxPlace++;
                    _pointPlaces[(sender.GetHashCode(), item.Id)] = index;
                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    while (layer >= _blocks.Count)
                    {
                        _blocks.Add(CreateNewBlock());
                    }
                    ProcessItem(_blocks[layer], item, inLayerId);
                }

                _amountOfItems = newAmountOfItems;
            }
        }

        /// <inheritdoc />
        public void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (_pointPlaces)
            {
                foreach (var item in e.UpdatedItems)
                {
                    var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    ProcessItem(_blocks[layer], item, inLayerId);
                }
            }
        }

        /// <inheritdoc />
        public void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            lock (_pointPlaces)
            {
                var removed = 0;
                foreach (var item in e.RemovedItems)
                {
                    if (!_pointPlaces.ContainsKey((sender.GetHashCode(), item.Id))) continue;

                    var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                    _pointPlaces.Remove((sender.GetHashCode(), item.Id));
                    if (index == _maxPlace - 1) _maxPlace--;
                    else _freePlaces.Enqueue(index);

                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    RemoveItem(_blocks[layer], inLayerId);
                    removed++;
                }

                _amountOfItems -= removed;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var block in _blocks)
            {
                block.Dispose();
            }

            _blocks.Clear();
            _pointPlaces.Clear();
        }

        #endregion

        #region Protected

        /// <summary> Adds item to block. </summary>
        /// <param name="block"> Block where item should be added. </param>
        /// <param name="item"> Item to add. </param>
        /// <param name="inBlockId"> Index of item in block. </param>
        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item, int inBlockId);

        /// <summary> Removes item from block </summary>
        /// <param name="block"> Block from where item should be removed. </param>
        /// <param name="inBlockId"> Index of item in block. </param>
        protected abstract void RemoveItem(TCloudBlock block, int inBlockId);

        /// <summary> How many items block can contain. </summary>
        protected abstract int BlockCapacity { get; }

        /// <summary> Adds new block of specific type. </summary>
        /// <returns> Created block. </returns>
        protected abstract TCloudBlock CreateNewBlock();

        /// <summary> Checks is object that sends update event is visible. </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        protected static bool IsSenderVisible(object sender) => (sender as IVisibleDataSource)?.IsVisible ?? true;

        /// <summary> Shader for rendering this block. </summary>
        /// <remarks> It should handle compute buffer. </remarks>
        protected readonly Shader Shader;

        #endregion

        #region Private definitions

        private readonly Dictionary<(int, int), int> _pointPlaces = new Dictionary<(int, int), int>();
        private readonly Queue<int> _freePlaces = new Queue<int>();
        private readonly List<TCloudBlock> _blocks = new List<TCloudBlock>();
        private int _maxPlace = 0;
        private int _amountOfItems;
        private float _scale;

        #endregion
    }
}