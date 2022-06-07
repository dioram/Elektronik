using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;

namespace Elektronik.DataSources.Containers
{
    /// <summary> Base implementation of cloud container. </summary>
    /// <typeparam name="TCloudItem"></typeparam>
    public class CloudContainerBase<TCloudItem> : ICloudContainer<TCloudItem>
            where TCloudItem : struct, ICloudItem
    {
        /// <summary> Constructor. </summary>
        /// <param name="displayName"> Name that will be displayed in tree. </param>
        public CloudContainerBase(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? typeof(TCloudItem).Name : displayName;
        }
        
        #region IDataSource

        /// <inheritdoc />
        public string DisplayName { get; set; }

        /// <inheritdoc />
        public IEnumerable<IDataSource> Children => Enumerable.Empty<IDataSource>();

        /// <inheritdoc />
        public virtual void AddConsumer(IDataConsumer consumer)
        {
            if (!(consumer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            OnAdded += typedRenderer.OnItemsAdded;
            OnUpdated += typedRenderer.OnItemsUpdated;
            OnRemoved += typedRenderer.OnItemsRemoved;
            if (Count > 0)
            {
                typedRenderer.OnItemsAdded(this, new AddedEventArgs<TCloudItem>(this));
            }
        }

        /// <inheritdoc />
        public virtual void RemoveConsumer(IDataConsumer consumer)
        {
            if (!(consumer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            OnAdded -= typedRenderer.OnItemsAdded;
            OnUpdated -= typedRenderer.OnItemsUpdated;
            OnRemoved -= typedRenderer.OnItemsRemoved;
        }

        /// <inheritdoc />
        public IDataSource TakeSnapshot()
        {
            var res = new CloudContainer<TCloudItem>(DisplayName);
            List<TCloudItem> list;
            lock (Items)
            {
                list = Items.Values.ToList();
            }

            res.AddRange(list);
            return res;
        }

        #endregion
        
        #region ICloudContainer

        /// <inheritdoc />
        public IEnumerator<TCloudItem> GetEnumerator()
        {
            lock (Items)
            {
                return Items.Values.GetEnumerator();
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public virtual void Add(TCloudItem item)
        {
            lock (Items)
            {
                Items.Add(item.Id, item);
                OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(item));
            }
        }

        /// <inheritdoc />
        public virtual bool Contains(TCloudItem item) => Contains(item.Id);

        /// <inheritdoc />
        public virtual bool Contains(int id)
        {
            lock (Items)
            {
                return Items.ContainsKey(id);
            }
        }

        /// <inheritdoc />
        public virtual void CopyTo(TCloudItem[] array, int arrayIndex)
        {
            lock (Items)
            {
                Items.Values.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public virtual bool Remove(TCloudItem item)
        {
            bool res;
            lock (Items)
            {
                res = Items.Remove(item.Id);
            }
            if (res) OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(item));

            return res;
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (Items)
                {
                    return Items.Count;
                }
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        /// <remarks> Actually does nothing, just returns item's id. </remarks>
        public virtual int IndexOf(TCloudItem item) => item.Id;

        /// <inheritdoc />
        public virtual void Insert(int index, TCloudItem item) => Add(item);

        /// <inheritdoc />
        public virtual void RemoveAt(int index)
        {
            TCloudItem item;
            lock (Items)
            {
                if (!Items.ContainsKey(index)) return;
                item = Items[index];
                Items.Remove(index);
            }
            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(item));
        }

        public TCloudItem this[int index]
        {
            get
            {
                lock (Items)
                {
                    return Items[index];
                }
            }
            set
            {
                bool contains;
                lock (Items)
                {
                    contains = Items.ContainsKey(index);
                }

                if (contains) Update(value);
                else Add(value);
            }
        }

        /// <inheritdoc />
        public event EventHandler<AddedEventArgs<TCloudItem>> OnAdded;

        /// <inheritdoc />
        public event EventHandler<UpdatedEventArgs<TCloudItem>> OnUpdated;

        /// <inheritdoc />
        public event EventHandler<RemovedEventArgs<TCloudItem>> OnRemoved;

        /// <inheritdoc />
        public virtual void AddRange(IList<TCloudItem> items)
        {
            if (items is null) return;
            lock (Items)
            {
                foreach (var ci in items)
                {
                    Items.Add(ci.Id, ci);
                }

                OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(items));
            }
        }

        /// <inheritdoc />
        public virtual void Remove(IList<TCloudItem> items)
        {
            if (items is null) return;
            lock (Items)
            {
                foreach (var ci in items)
                {
                    Items.Remove(ci.Id);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(items));
        }

        /// <inheritdoc />
        public IList<TCloudItem> Remove(IList<int> itemIds)
        {
            if (itemIds is null) return new List<TCloudItem>();
            var removed = new List<TCloudItem> { Capacity = itemIds.Count };
            lock (Items)
            {
                foreach (var ci in itemIds.Where(i => Items.ContainsKey(i)))
                {
                    removed.Add(Items[ci]);
                    Items.Remove(ci);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(removed));
            return removed;
        }

        /// <inheritdoc cref="IDataSource.Clear" />
        public virtual void Clear()
        {
            lock (Items)
            {
                var list = Items.Values.ToArray();
                Items.Clear();
                OnRemoved?.Invoke(this, new RemovedEventArgs<TCloudItem>(list));
            }
        }

        /// <inheritdoc />
        public virtual void Update(TCloudItem item)
        {
            lock (Items)
            {
                if (!Items.ContainsKey(item.Id)) return;
                Items[item.Id] = item;
                OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(item));
            }
        }

        /// <inheritdoc />
        public virtual void Update(IList<TCloudItem> items)
        {
            if (items is null) return;
            lock (Items)
            {
                foreach (var ci in items.Where(ci => Items.ContainsKey(ci.Id)))
                {
                    Items[ci.Id] = ci;
                }

                OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(items));
            }
        }
        
        #endregion

        #region Protected
        
        // I don't remember why it is using sorted dictionary.
        // TODO: Change to dictionary, test and measure performance.
        
        /// <summary> Cloud items by their Ids. </summary>
        protected readonly SortedDictionary<int, TCloudItem> Items = new SortedDictionary<int, TCloudItem>();

        #endregion
    }
}