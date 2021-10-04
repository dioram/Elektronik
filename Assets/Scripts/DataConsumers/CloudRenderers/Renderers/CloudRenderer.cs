using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base class for rendering object clouds. Such as point cloud, line cloud, etc. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    /// <typeparam name="TCloudBlock"></typeparam>
    /// <typeparam name="TGpuItem"></typeparam>
    public abstract class CloudRenderer<TCloudItem, TCloudBlock, TGpuItem>
            : ICloudRenderer<TCloudItem>, IQueueableRenderer
            where TCloudItem : struct, ICloudItem
            where TCloudBlock : class, ICloudBlock<TGpuItem>
    {
        public CloudRenderer(Shader shader)
        {
            Shader = shader;
        }

        public IReadOnlyCollection<TCloudBlock> Blocks => _blocks.AsReadOnly();
        
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

        #region IQueueableRenderer

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

        public int RenderQueue => _blocks.Count > 0 ? _blocks[0].RenderQueue : 0;

        #endregion

        #region ICloudRenderer

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

        public float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;

                foreach (var block in _blocks)
                {
                    block.Scale = value;
                }
            }
        }

        public void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            var addedItems = Filter is null ? e.AddedItems : e.AddedItems.Where(Filter).ToArray();
            var newAmountOfItems = _amountOfItems + addedItems.Count;
            lock (_pointPlaces)
            {
                while (newAmountOfItems > _blocks.Count * BlockCapacity)
                {
                    _blocks.Add(CreateNewBlock());
                }

                foreach (var item in addedItems)
                {
                    var index = _freePlaces.Count > 0 ? _freePlaces.Dequeue() : _maxPlace++;
                    _pointPlaces[(sender.GetHashCode(), item.Id)] = index;
                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    ProcessItem(_blocks[layer], item, inLayerId);
                }

                _amountOfItems = newAmountOfItems;
            }
        }

        public void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            var updatedItems = Filter is null ? e.UpdatedItems : e.UpdatedItems.Where(Filter);
            lock (_pointPlaces)
            {
                foreach (var item in updatedItems)
                {
                    var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    ProcessItem(_blocks[layer], item, inLayerId);
                }
            }
        }

        public void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            var removedItems = Filter is null ? e.RemovedItems : e.RemovedItems.Where(Filter).ToList();
            lock (_pointPlaces)
            {
                foreach (var item in removedItems)
                {
                    if (!_pointPlaces.ContainsKey((sender.GetHashCode(), item.Id))) continue;

                    var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                    _pointPlaces.Remove((sender.GetHashCode(), item.Id));
                    if (index == _maxPlace - 1) _maxPlace--;

                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    RemoveItem(_blocks[layer], inLayerId);
                }

                _amountOfItems -= removedItems.Count;
            }
        }

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

        #region Protected definitions

        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item, int inBlockId);

        protected abstract void RemoveItem(TCloudBlock block, int inBlockId);

        protected abstract int BlockCapacity { get; }

        [CanBeNull] protected virtual Func<TCloudItem, bool> Filter { get; } = null;

        protected abstract TCloudBlock CreateNewBlock();

        protected static bool IsSenderVisible(object sender) => (sender as IVisible)?.IsVisible ?? true;

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