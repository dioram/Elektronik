using System;
using System.Collections.Generic;

namespace Elektronik.Common.Clouds.V2
{
    public interface IFastCloud<TCloudItem> where TCloudItem: ICloudItem
    {
        event Action<IEnumerable<TCloudItem>> ItemsAdded;
        
        event Action<IEnumerable<TCloudItem>> ItemsUpdated;
        
        event Action<IEnumerable<int>> ItemsRemoved;

        event Action ItemsCleared; 
        
        int Count { get; }
        
        void Clear();
        
        bool Exists(int idx);
        
        TCloudItem Get(int idx);
        
        void Add(TCloudItem item);
        
        void AddRange(IEnumerable<TCloudItem> items);
        
        void UpdateItem(TCloudItem item);
        
        void UpdateItems(IEnumerable<TCloudItem> items);
        
        void RemoveAt(int idx);
        
        void RemoveAt(IEnumerable<int> itemsIds);
        
        void SetActive(bool value);
    }
}