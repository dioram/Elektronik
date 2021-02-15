using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds.V2
{
    
    public abstract class FastCloud<TCloudItem> 
            : MonoBehaviour, IFastCloud<TCloudItem> 
            where TCloudItem: ICloudItem
    {
        #region IFastCloud implementation

        public int Count => Items.Count();

        public event Action<IEnumerable<TCloudItem>> ItemsAdded;
        public event Action<IEnumerable<TCloudItem>> ItemsUpdated;
        public event Action<IEnumerable<int>> ItemsRemoved;
        public event Action ItemsCleared;

        public void Clear()
        {
            Items.Clear();
            ItemsCleared?.Invoke();
        }

        public bool Exists(int idx)
        {
            return Items.ContainsKey(idx);
        }

        public TCloudItem Get(int id)
        {
            return Items[id];
        }

        public void Add(TCloudItem item)
        {
            Items.Add(item.Id, item);
            ItemsAdded?.Invoke(new []{item});
        }

        public void AddRange(IEnumerable<TCloudItem> items)
        {
            foreach (var item in items)
            {
                Items.Add(item.Id, item);
            }
            ItemsAdded?.Invoke(items);
        }

        public void UpdateItem(TCloudItem item)
        {
            Items[item.Id] = item;
            ItemsUpdated?.Invoke(new []{item});
        }

        public void UpdateItems(IEnumerable<TCloudItem> items)
        {
            foreach (var item in items)
            {
                Items[item.Id] = item;
            }
            ItemsUpdated?.Invoke(items);
        }

        public void RemoveAt(int idx)
        {
            Items.Remove(idx);
            ItemsRemoved?.Invoke(new []{idx});
        }

        public void RemoveAt(IEnumerable<int> pointsIds)
        {
            foreach (var id in pointsIds)
            {
                Items.Remove(id);
            }
            ItemsRemoved?.Invoke(pointsIds);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        #endregion

        #region Protected definitions

        protected Dictionary<int, TCloudItem> Items = new Dictionary<int, TCloudItem>();

        #endregion

    }
}