using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public abstract class CloudRenderer<TCloudItem, TCloudBlock>
            : MonoBehaviour
            where TCloudItem : ICloudItem, new()
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

        #region Container changes handlers

        public void OnItemsAdded(IContainer<TCloudItem> container, IEnumerable<TCloudItem> items)
        {
            _amountOfItems += items.Count();
            if (_amountOfItems > (_blocks.Count - 1) * CloudBlock.Capacity)
            {
                _needNewBlock = true;
            }
            
            foreach (var item in items)
            {
                var index = _freePlaces.Count > 0 ? _freePlaces.Dequeue() : _maxPlace++;
                _pointPlaces.Add((container.GetHashCode(), item.Id), index);
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item, inLayerId);
                _blocks[layer].Updated = true;
            }
        }

        public void OnItemsUpdated(IContainer<TCloudItem> container, IEnumerable<TCloudItem> items)
        {
            foreach (var item in items)
            {
                var index = _pointPlaces[(container.GetHashCode(), item.Id)];
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item, inLayerId);
                _blocks[layer].Updated = true;
            }
        }

        public void OnItemsRemoved(IContainer<TCloudItem> container, IEnumerable<int> removedItemsIds)
        {
            foreach (var itemId in removedItemsIds)
            {
                if (!_pointPlaces.ContainsKey((container.GetHashCode(), itemId))) continue;
                
                var index = _pointPlaces[(container.GetHashCode(), itemId)];
                _pointPlaces.Remove((container.GetHashCode(), itemId));
                if (index == _maxPlace - 1) _maxPlace--;
                else _freePlaces.Enqueue(index);
                
                int layer = index / CloudBlock.Capacity;
                int inLayerId = index % CloudBlock.Capacity;
                RemoveItem(_blocks[layer], inLayerId);
                _blocks[layer].Updated = true;
            }

            _amountOfItems -= removedItemsIds.Count();
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