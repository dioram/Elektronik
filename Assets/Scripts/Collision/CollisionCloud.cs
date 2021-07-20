using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Collision
{
    public class CollisionCloud<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        public float Radius;
        
        // for Unity editor binding.
        public void SetRadius(float value)
        {
            Radius = value;
        }

        public (IContainer<TCloudItem> container, TCloudItem item)? FindCollided(Ray ray)
        {
            var id = _topBlock.FindItem(ray, Radius);
            if (!id.HasValue) return null;
            
            var (sender, senderId) = _dataReverse[id.Value];
            var container = sender as IContainer<TCloudItem>;
            if (container == null || !container.Contains(senderId)) return null;
            var item = container[senderId];
            return (container, item);
        }
        
        #region ICloudRenderer

        public void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            _threadWorker.Enqueue(() =>
            {
                lock (_data)
                {
                    foreach (var item in e.AddedItems)
                    {
                        var id = _maxId;
                        var pos = item.AsPoint().Position;
                        _data[(sender, item.Id)] =  (id, pos);
                        _dataReverse[id] = (sender, item.Id);
                        _topBlock.AddItem(id, pos);
                        _maxId++;
                    }
                }
            });
        }

        public void OnItemsUpdated(object sender, UpdatedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            _threadWorker.Enqueue(() =>
            {
                lock (_data)
                {
                    foreach (var item in e.UpdatedItems)
                    {
                        var key = (sender, item.Id);
                        var newPos = item.AsPoint().Position;
                        if (!_data.ContainsKey(key)) continue;
                        var (id, oldPos) = _data[key];
                        _data[key] = (id, newPos);
                        _topBlock.UpdateItem(id, oldPos, newPos);
                    }
                }
            });
        }

        public void OnItemsRemoved(object sender, RemovedEventArgs e)
        {
            _threadWorker.Enqueue(() =>
            {
                lock (_data)
                {
                    foreach (var senderId in e.RemovedIds)
                    {
                        var key = (sender, senderId);
                        if (!_data.ContainsKey(key)) continue;
                        var (id, pos) = _data[key];
                        _data.Remove(key);
                        _dataReverse.Remove(id);
                        _topBlock.RemoveItem(id, pos);
                    }
                }
            });
        }

        public void ShowItems(object sender, IEnumerable<TCloudItem> items)
        {
            OnClear(sender);
            OnItemsAdded(sender, new AddedEventArgs<TCloudItem>(items));
        }

        public void OnClear(object sender)
        {
            List<int> keys;
            lock (_data)
            {
                keys = _data.Keys.Where(k => k.sender == sender).Select(k => k.id).ToList();
            }
            OnItemsRemoved(sender, new RemovedEventArgs(keys));
        }

        #endregion

        #region Private

        private readonly CollisionBlock _topBlock = new CollisionBlock(Vector3Int.zero);

        private readonly Dictionary<(object sender, int id), (int id, Vector3 pos)> _data =
                new Dictionary<(object sender, int id), (int id, Vector3 pos)>();

        private readonly Dictionary<int, (object sender, int id)> _dataReverse =
                new Dictionary<int, (object sender, int id)>();

        private int _maxId = 0;
        private readonly ThreadWorker _threadWorker = new ThreadWorker();
        
        private static bool IsSenderVisible(object sender) => (sender as IVisible)?.IsVisible ?? true;
        
        #endregion
    }
}