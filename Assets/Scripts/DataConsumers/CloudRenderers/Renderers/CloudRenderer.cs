using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Grid = Elektronik.Cameras.Grid;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Base class for rendering object clouds. Such as point cloud, line cloud, etc. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    /// <typeparam name="TCloudBlock"></typeparam>
    /// <typeparam name="TGpuItem"></typeparam>
    public abstract class CloudRenderer<TCloudItem, TCloudBlock, TGpuItem>
            : CloudRendererComponent<TCloudItem>, IQueueableRenderer
            where TCloudItem : struct, ICloudItem
            where TCloudBlock : class, ICloudBlock<TGpuItem>
    {
        public const string GridMessage = "Grid";
        public Grid Grid;
        public Shader CloudShader;

        public override int ItemsCount
        {
            get
            {
                lock (_pointPlaces)
                {
                    return _pointPlaces.Count;
                }
            }
        }

        #region Unity events

        private void Start()
        {
            lock (_pointPlaces)
            {
                Blocks.Add(CreateNewBlock());
            }
        }

        private void Update()
        {
            lock (_pointPlaces)
            {
                foreach (var block in Blocks)
                {
                    block.UpdateDataOnGPU();
                }
            }
        }

        #endregion

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

        public int RenderQueue => Blocks.Count > 0 ? Blocks[0].RenderQueue : 0;

        #endregion

        #region ICloudRenderer

        public override float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;

                foreach (var block in Blocks)
                {
                    block.Scale = value;
                }
            }
        }

        public override void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            var addedItems = Filter is null ? e.AddedItems : e.AddedItems.Where(Filter).ToArray();
            var newAmountOfItems = _amountOfItems + addedItems.Count;
            lock (_pointPlaces)
            {
                while (newAmountOfItems > Blocks.Count * BlockCapacity)
                {
                    Blocks.Add(CreateNewBlock());
                }

                foreach (var item in addedItems)
                {
                    if (item.Message == GridMessage && item is SlamPlane plane)
                    {
                        MainThreadInvoker.Enqueue(() => Grid.SetPlane(plane));
                        continue;
                    }

                    var index = _freePlaces.Count > 0 ? _freePlaces.Dequeue() : _maxPlace++;
                    _pointPlaces[(sender.GetHashCode(), item.Id)] = index;
                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    ProcessItem(Blocks[layer], item, inLayerId);
                }

                _amountOfItems = newAmountOfItems;
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            var updatedItems = Filter is null ? e.UpdatedItems : e.UpdatedItems.Where(Filter);
            lock (_pointPlaces)
            {
                foreach (var item in updatedItems)
                {
                    if (item.Message == GridMessage && item is SlamPlane) continue;
                    var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    ProcessItem(Blocks[layer], item, inLayerId);
                }
            }
        }

        public override void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
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
                    else _freePlaces.Enqueue(index);

                    var layer = index / BlockCapacity;
                    var inLayerId = index % BlockCapacity;
                    RemoveItem(Blocks[layer], inLayerId);
                }

                _amountOfItems -= removedItems.Count;
            }
        }

        #endregion

        #region Protected definitions

        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item, int inBlockId);

        protected abstract void RemoveItem(TCloudBlock block, int inBlockId);

        protected readonly List<TCloudBlock> Blocks = new List<TCloudBlock>();

        protected abstract int BlockCapacity { get; }

        [CanBeNull] protected virtual Func<TCloudItem, bool> Filter { get; } = null;

        protected abstract TCloudBlock CreateNewBlock();

        #endregion

        #region Private definitions

        private readonly Dictionary<(int, int), int> _pointPlaces = new Dictionary<(int, int), int>();
        private readonly Queue<int> _freePlaces = new Queue<int>();
        private int _maxPlace = 0;
        private int _amountOfItems;
        private float _scale;

        #endregion
    }
}