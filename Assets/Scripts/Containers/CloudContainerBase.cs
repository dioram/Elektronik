using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers
{
    public class CloudContainerBase<TCloudItem> : IContainer<TCloudItem>, ISourceTree
            where TCloudItem : struct, ICloudItem
    {
        #region ISourceTree

        public string DisplayName { get; set; }

        public IEnumerable<ISourceTree> Children => Enumerable.Empty<ISourceTree>();

        public virtual void SetRenderer(ISourceRenderer renderer)
        {
            if (!(renderer is ICloudRenderer<TCloudItem> typedRenderer)) return;
            OnAdded += typedRenderer.OnItemsAdded;
            OnUpdated += typedRenderer.OnItemsUpdated;
            OnRemoved += typedRenderer.OnItemsRemoved;
            if (Count > 0)
            {
                typedRenderer.OnItemsAdded(this, new AddedEventArgs<TCloudItem>(this));
            }
        }

        #endregion
        
        #region IContainer

        public IEnumerator<TCloudItem> GetEnumerator()
        {
            lock (Items)
            {
                return Items.Values.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual void Add(TCloudItem item)
        {
            lock (Items)
            {
                Items.Add(item.Id, item);
                OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(item));
            }
        }

        public virtual bool Contains(TCloudItem item) => Contains(item.Id);

        public virtual bool Contains(int id)
        {
            lock (Items)
            {
                return Items.ContainsKey(id);
            }
        }

        public virtual void CopyTo(TCloudItem[] array, int arrayIndex)
        {
            lock (Items)
            {
                Items.Values.CopyTo(array, arrayIndex);
            }
        }

        public virtual bool Remove(TCloudItem item)
        {
            lock (Items)
            {
                var res = Items.Remove(item.Id);
                OnRemoved?.Invoke(this, new RemovedEventArgs(item.Id));

                return res;
            }
        }

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

        public bool IsReadOnly => false;

        public virtual int IndexOf(TCloudItem item) => item.Id;

        public virtual void Insert(int index, TCloudItem item) => Add(item);

        public virtual void RemoveAt(int index)
        {
            lock (Items)
            {
                Items.Remove(index);
                OnRemoved?.Invoke(this, new RemovedEventArgs(index));
            }
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

        public event EventHandler<AddedEventArgs<TCloudItem>> OnAdded;
        public event EventHandler<UpdatedEventArgs<TCloudItem>> OnUpdated;
        public event EventHandler<RemovedEventArgs> OnRemoved;
        
        public virtual void AddRange(IEnumerable<TCloudItem> items)
        {
            if (items is null) return;
            lock (Items)
            {
                var list = items.ToList();
                foreach (var ci in list)
                {
                    Items.Add(ci.Id, ci);
                }

                OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(list));
            }
        }

        public virtual void Remove(IEnumerable<TCloudItem> items)
        {
            if (items is null) return;
            var list = items.ToList();
            lock (Items)
            {
                foreach (var ci in list)
                {
                    Items.Remove(ci.Id);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(p => p.Id).ToList()));
        }

        public IEnumerable<TCloudItem> Remove(IEnumerable<int> itemIds)
        {
            if (itemIds is null) return new List<TCloudItem>();
            var list = itemIds.ToList();
            var removed = new List<TCloudItem> { Capacity = list.Count };
            lock (Items)
            {
                foreach (var ci in list.Where(i => Items.ContainsKey(i)))
                {
                    removed.Add(Items[ci]);
                    Items.Remove(ci);
                }
            }

            OnRemoved?.Invoke(this, new RemovedEventArgs(list));
            return removed;
        }
        
        public virtual void Clear()
        {
            lock (Items)
            {
                var ids = Items.Keys.ToList();
                Items.Clear();
                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        public virtual void Update(TCloudItem item)
        {
            lock (Items)
            {
                if (!Items.ContainsKey(item.Id)) return;
                Items[item.Id] = item;
                OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(item));
            }
        }

        public virtual void Update(IEnumerable<TCloudItem> items)
        {
            if (items is null) return;
            lock (Items)
            {
                var list = items.ToList();
                foreach (var ci in list.Where(ci => Items.ContainsKey(ci.Id)))
                {
                    Items[ci.Id] = ci;
                }

                OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(list));
            }
        }
        
        #endregion

        #region Protected
        
        protected readonly SortedDictionary<int, TCloudItem> Items = new SortedDictionary<int, TCloudItem>();

        #endregion
    }
}