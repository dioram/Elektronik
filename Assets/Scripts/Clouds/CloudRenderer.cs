using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    /// <summary> Base class for rendering object clouds. Such as point cloud, line cloud, etc. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    /// <typeparam name="TCloudBlock"></typeparam>
    public abstract class CloudRenderer<TCloudItem, TCloudBlock>
            : CloudRendererComponent<TCloudItem>
            where TCloudItem : struct, ICloudItem
            where TCloudBlock : CloudBlock
    {
        public Shader CloudShader;

        public float ItemSize;
        
        public void SetSize(float newSize)
        {
            ItemSize = newSize;

            foreach (var block in _blocks)
            {
                block.ItemSize = ItemSize;
            }
        }

        #region Unity events

        private void Start()
        {
            _needNewBlock = true;
        }

        private void Update()
        {
            if (_needNewBlock)
            {
                var go = new GameObject($"{GetType().ToString().Split('.').Last()} {_blocks.Count}");
                go.transform.SetParent(transform);
                var block = go.AddComponent<TCloudBlock>();
                block.CloudShader = CloudShader;
                block.Updated = true;
                _blocks.Add(block);
                _needNewBlock = false;
            }

            foreach (var block in _blocks)
            {
                block.ItemSize = ItemSize;
            }
        }

        #endregion

        #region ICloudRenderer implementation

        public override void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e)
        {
            _amountOfItems += e.AddedItems.Count();
            if (_amountOfItems > (_blocks.Count - 1) * CloudBlock.Capacity)
            {
                _needNewBlock = true;
            }
            
            foreach (var item in e.AddedItems)
            {
                var index = _freePlaces.Count > 0 ? _freePlaces.Dequeue() : _maxPlace++;
                _pointPlaces.Add((sender.GetHashCode(), item.Id), index);
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item, inLayerId);
                _blocks[layer].Updated = true;
            }
        }

        public override void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e)
        {
            foreach (var item in e.UpdatedItems)
            {
                var index = _pointPlaces[(sender.GetHashCode(), item.Id)];
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item, inLayerId);
                _blocks[layer].Updated = true;
            }
        }

        public override void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e)
        {
            foreach (var itemId in e.RemovedIds)
            {
                if (!_pointPlaces.ContainsKey((sender.GetHashCode(), itemId))) continue;
                
                var index = _pointPlaces[(sender.GetHashCode(), itemId)];
                _pointPlaces.Remove((sender.GetHashCode(), itemId));
                if (index == _maxPlace - 1) _maxPlace--;
                else _freePlaces.Enqueue(index);
                
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                RemoveItem(_blocks[layer], inLayerId);
                _blocks[layer].Updated = true;
            }

            _amountOfItems -= e.RemovedIds.Count();
        }

        #endregion

        #region Protected definitions

        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item, int inBlockId);

        protected abstract void RemoveItem(TCloudBlock block, int inBlockId);

        #endregion

        #region Private definitions

        private readonly List<TCloudBlock> _blocks = new List<TCloudBlock>();
        private readonly Dictionary<(int, int), int> _pointPlaces = new Dictionary<(int, int), int>();
        private readonly Queue<int> _freePlaces = new Queue<int>();
        private int _maxPlace = 0;
        private bool _needNewBlock;
        private int _amountOfItems;

        #endregion
    }
}