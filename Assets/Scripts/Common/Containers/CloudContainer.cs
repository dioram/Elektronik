using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public class CloudContainer<TCloudItem>: IContainer<TCloudItem>, IContainerTree
        where TCloudItem: struct, ICloudItem
    {
        public CloudContainer(string displayName = "")
        {
            DisplayName = string.IsNullOrEmpty(displayName) ? typeof(TCloudItem).Name : displayName;
        }

        #region IContainer implementation

        public IEnumerator<TCloudItem> GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TCloudItem item)
        {
            _items.Add(item.Id, item);
            if (IsActive)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(new []{item}));
            }
        }
        
        public void Clear()
        {
            var ids = _items.Keys.ToArray();
            _items.Clear();
            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
            }
        }

        public bool Contains(TCloudItem item) => _items.ContainsKey(item.Id);

        public void CopyTo(TCloudItem[] array, int arrayIndex)
        {
            _items.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TCloudItem item)
        {
            var res = _items.Remove(item.Id);
            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new []{item.Id}));
            }
            return res;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public int IndexOf(TCloudItem item) => item.Id;

        public void Insert(int index, TCloudItem item) => Add(item);

        public void RemoveAt(int index)
        {
            _items.Remove(index);
            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(new []{index}));
            }
        }

        public TCloudItem this[int index]
        {
            get => _items[index];
            set
            {
                if (_items.ContainsKey(index))
                {
                    UpdateItem(value);
                }
                else
                {
                    Add(value);
                }
            }
        }

        public event Action<IContainer<TCloudItem>, AddedEventArgs<TCloudItem>> OnAdded;
        public event Action<IContainer<TCloudItem>, UpdatedEventArgs<TCloudItem>> OnUpdated;
        public event Action<IContainer<TCloudItem>, RemovedEventArgs> OnRemoved;

        public void AddRange(IEnumerable<TCloudItem> items)
        {
            var list = items.ToList();
            foreach (var ci in list)
            {
                _items.Add(ci.Id, ci);
            }

            if (IsActive)
            {
                OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(list));
            }
        }

        public void Remove(IEnumerable<TCloudItem> items)
        {
            var list = items.ToList();
            foreach (var ci in list)
            {
                _items.Remove(ci.Id);
            }

            if (IsActive)
            {
                OnRemoved?.Invoke(this, new RemovedEventArgs(list.Select(p => p.Id).ToList()));
            }
        }
        
        public void UpdateItem(TCloudItem item)
        {
            _items[item.Id] = item;
            if (IsActive)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(new []{item}));
            }
        }

        public void UpdateItems(IEnumerable<TCloudItem> items)
        {
            var list = items.ToList();
            foreach (var ci in list)
            {
                _items[ci.Id] = ci;
            }

            if (IsActive)
            {
                OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(list));
            }
        }

        #endregion

        #region IContainerTree implementations

        public string DisplayName { get; protected set; }

        public IEnumerable<IContainerTree> Children => Enumerable.Empty<IContainerTree>();

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                if (_isActive) OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(this));
                else OnRemoved?.Invoke(this, new RemovedEventArgs(_items.Keys.ToList()));
            }
        }

        public void SetRenderer(object renderer)
        {
            if (renderer is ICloudRenderer<TCloudItem> typedRenderer)
            {
                OnAdded += typedRenderer.OnItemsAdded;
                OnUpdated += typedRenderer.OnItemsUpdated;
                OnRemoved += typedRenderer.OnItemsRemoved;
                if (Count > 0)
                {
                    OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(this));
                }
            }
        }
        
        #endregion

        #region Private definitions

        private readonly SortedDictionary<int, TCloudItem> _items = new SortedDictionary<int, TCloudItem>();
        private bool _isActive = true;

        #endregion
    }
}