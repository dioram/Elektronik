using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds.V2
{
    public abstract class CloudRenderer<TCloudItem, TCloudBlock>
            : MonoBehaviour
            where TCloudItem : ICloudItem, new()
            where TCloudBlock : CloudBlock
    {
        public Shader CloudShader;

        public float ItemSize;

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

        public void OnItemsAdded(IEnumerable<TCloudItem> items)
        {
            if (AmountOfItems > (_blocks.Count - 1) * CloudBlock.Capacity)
            {
                _needNewBlock = true;
            }

            foreach (var item in items)
            {
                int layer = item.Id / CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item);
                _blocks[layer].Updated = true;
            }

            AmountOfItems += items.Count();
        }

        public void OnItemsUpdated(IEnumerable<TCloudItem> items)
        {
            foreach (var item in items)
            {
                int layer = item.Id / CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item);
                _blocks[layer].Updated = true;
            }
        }

        public void OnItemsRemoved(IEnumerable<int> removedItemsIds)
        {
            foreach (var itemId in removedItemsIds)
            {
                int layer = itemId / CloudBlock.Capacity;
                RemoveItem(_blocks[layer], itemId);
                _blocks[layer].Updated = true;
            }

            AmountOfItems -= removedItemsIds.Count();
        }

        public void OnItemsCleared()
        {
            foreach (var block in _blocks)
            {
                block.Clear();
            }

            AmountOfItems = 0;
        }

        #endregion

        #region Protected definitions

        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item);

        protected abstract void RemoveItem(TCloudBlock block, int id);

        #endregion

        #region Private definitions

        private readonly List<TCloudBlock> _blocks = new List<TCloudBlock>();
        private bool _needNewBlock;
        private int AmountOfItems;

        #endregion
    }
}