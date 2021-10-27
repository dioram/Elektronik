using System.Collections.Generic;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.Threading;
using UnityEngine;

namespace Elektronik.DataConsumers.Collision
{
    /// <summary> Class that implements custom collisions between rays and cloud object. </summary>
    /// <remarks>
    /// <para>
    /// This is necessary because we can't check collision using <c>UnityEngine.Physics</c> without instantiating objects
    /// and we can't instantiate thousands of objects because it will be critical performance hit.
    /// </para>
    /// <para>
    /// Since Unity can't instantiate generic types this class was made abstract.
    /// If you want to instantiate it you need derive specialisation of this class.
    /// </para>
    /// </remarks>
    /// <typeparam name="TCloudItem"> Type of supported cloud data. </typeparam>
    public abstract class CollisionCloud<TCloudItem> : MonoBehaviour, ICloudRenderer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        #region Editor fields
        
        /// <summary> Multiplier for radius of collision sphere. </summary>
        /// <remarks> It is used to set different collision sphere radius for different types of objects. </remarks>
        [SerializeField]
        [Tooltip("Multiplier for radius of collision sphere.\n" +
            "It is used to set different collision sphere radius for different types of objects.")]
        private float RadiusMultiplier = 1;

        #endregion

        /// <summary> Radius of collision sphere around object. </summary>
        public float Radius { get; set; } = 1;
        
        /// <summary> Finds first object that colliding with ray. </summary>
        /// <param name="ray"> </param>
        /// <returns> Tuple of collided object and its container or null if no collided objects were found. </returns>
        public (ICloudContainer<TCloudItem> container, TCloudItem item)? FindCollided(Ray ray)
        {
            ray.origin /= Scale;
            var id = _topBlock.FindItem(ray, Radius * RadiusMultiplier / Scale);
            if (!id.HasValue) return null;

            var (sender, senderId) = _dataReverse[id.Value];
            if (!(sender is ICloudContainer<TCloudItem> container) || !container.Contains(senderId)) return null;
            var item = container[senderId];
            return (container, item);
        }

        #region ICloudRenderer

        /// <summary> Scale of the scene. </summary>
        public float Scale { get; set; } = 1;

        /// <inheritdoc />
        public int ItemsCount { get; private set; }

        /// <inheritdoc />
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
                        var pos = item.ToPoint().Position;
                        _data[(sender, item.Id)] = (id, pos);
                        _dataReverse[id] = (sender, item.Id);
                        _topBlock.AddItem(id, pos);
                        _maxId++;
                    }
                }
            });
            ItemsCount += e.AddedItems.Count;
        }

        /// <inheritdoc />
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
                        var newPos = item.ToPoint().Position;
                        if (!_data.ContainsKey(key)) continue;
                        var (id, oldPos) = _data[key];
                        _data[key] = (id, newPos);
                        _topBlock.UpdateItem(id, oldPos, newPos);
                    }
                }
            });
        }

        /// <inheritdoc />
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
            ItemsCount -= e.RemovedItems.Count;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _data.Clear();
            _dataReverse.Clear();
            _topBlock.Clear();
            _threadQueueWorker.Dispose();
        }

        #endregion

        #region Unity events

        private void OnDestroy()
        {
            Dispose();
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

        private static bool IsSenderVisible(object sender) => (sender as IVisibleDataSource)?.IsVisible ?? true;

        #endregion
    }
}