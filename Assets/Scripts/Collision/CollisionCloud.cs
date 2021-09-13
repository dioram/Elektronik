using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data.PackageObjects;
using Elektronik.Threading;
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
            ray.origin /= Scale;
            var id = _topBlock.FindItem(ray, Radius);
            if (!id.HasValue) return null;
            
            var (sender, senderId) = _dataReverse[id.Value];
            if (!(sender is IContainer<TCloudItem> container) || !container.Contains(senderId)) return null;
            var item = container[senderId];
            return (container, item);
        }

        #region Unity events

        private void OnDestroy()
        {
            _threadQueueWorker.Dispose();
        }

        #endregion
        
        #region ICloudRenderer
        
        public float Scale { get; set; }

        public void OnItemsAdded(object sender, AddedEventArgs<TCloudItem> e)
        {
            if (!IsSenderVisible(sender)) return;
            _threadQueueWorker.Enqueue(() =>
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
            _threadQueueWorker.Enqueue(() =>
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

        public void OnItemsRemoved(object sender, RemovedEventArgs<TCloudItem> e)
        {
            _threadQueueWorker.Enqueue(() =>
            {
                lock (_data)
                {
                    foreach (var item in e.RemovedItems)
                    {
                        var key = (sender, item.Id);
                        if (!_data.ContainsKey(key)) continue;
                        var (id, pos) = _data[key];
                        _data.Remove(key);
                        _dataReverse.Remove(id);
                        _topBlock.RemoveItem(id, pos);
                    }
                }
            });
        }

        #endregion

        #region Private

        private readonly CollisionBlock _topBlock = new CollisionBlock(Vector3Int.zero);

        private readonly Dictionary<(object sender, int id), (int id, Vector3 pos)> _data =
                new Dictionary<(object sender, int id), (int id, Vector3 pos)>();

        private readonly Dictionary<int, (object sender, int id)> _dataReverse =
                new Dictionary<int, (object sender, int id)>();

        private int _maxId = 0;
        private readonly ThreadQueueWorker _threadQueueWorker = new ThreadQueueWorker();
        
        private static bool IsSenderVisible(object sender) => (sender as IVisible)?.IsVisible ?? true;
        
        #endregion
    }
}