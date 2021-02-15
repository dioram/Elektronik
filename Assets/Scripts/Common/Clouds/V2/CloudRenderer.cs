using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Clouds.V2
{
    public abstract class CloudRenderer<TCloudItem, TCloudBlock>
            : MonoBehaviour
            where TCloudItem : Clouds.ICloudItem, new()
            where TCloudBlock : CloudBlock
    {
        public FastCloud<TCloudItem> Container;
        public Shader CloudShader;

        public float ItemSize;

        #region Unity events

        private void Start()
        {
            Container.ItemsAdded += OnItemsAdded;
            Container.ItemsUpdated += OnItemsUpdated;
            Container.ItemsRemoved += OnItemsRemoved;
            Container.ItemsCleared += OnItemsCleared;
            _needNewBlock = true;
        }

        private void Update()
        {
            if (_needNewBlock)
            {
                var go = new GameObject($"Item cloud block {_blocks.Count}");
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

        private void OnItemsAdded(IEnumerable<TCloudItem> items)
        {
            if (Container.Count > (_blocks.Count - 1) * CloudBlock.Capacity)
            {
                _needNewBlock = true;
            }

            foreach (var item in items)
            {
                int layer = item.Id / CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item);
                _blocks[layer].Updated = true;
            }
        }

        private void OnItemsUpdated(IEnumerable<TCloudItem> items)
        {
            foreach (var item in items)
            {
                int layer = item.Id / CloudBlock.Capacity;
                ProcessItem(_blocks[layer], item);
                _blocks[layer].Updated = true;
            }
        }

        private void OnItemsRemoved(IEnumerable<int> removedItemsIds)
        {
            foreach (var itemId in removedItemsIds)
            {
                int layer = itemId / CloudBlock.Capacity;
                ProcessItem(_blocks[layer], new TCloudItem {Id = itemId});
                _blocks[layer].Updated = true;
            }
        }

        private void OnItemsCleared()
        {
            foreach (var block in _blocks)
            {
                block.ToClear = true;
            }
        }

        #endregion

        #region Protected definitions

        protected abstract void ProcessItem(TCloudBlock block, TCloudItem item);

        #endregion

        #region Private definitions

        private readonly List<TCloudBlock> _blocks = new List<TCloudBlock>();
        private bool _needNewBlock;

        #endregion
    }
}