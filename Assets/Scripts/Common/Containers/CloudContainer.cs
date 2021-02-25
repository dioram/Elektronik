using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class CloudContainer<TCloudItem>: MonoBehaviour, IContainer<TCloudItem>
        where TCloudItem: struct, ICloudItem
    {
        public CloudRendererComponent<TCloudItem> Renderer;
        
        #region Unity events

        private void Start()
        {
            if (Renderer == null)
            {
                Debug.LogWarning($"No renderer set for {name}({GetType()})");
            }
            else
            {
                OnAdded += Renderer.OnItemsAdded;
                OnUpdated += Renderer.OnItemsUpdated;
                OnRemoved += Renderer.OnItemsRemoved;
            }
        }

        private void OnEnable()
        {
            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(this));
        }

        private void OnDisable()
        {
            OnRemoved?.Invoke(this, new RemovedEventArgs(_items.Keys));
        }

        private void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region IContainer implementation

        public IEnumerator<TCloudItem> GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TCloudItem item)
        {
            _items.Add(item.Id, item);
            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(new []{item}));
        }

        public void Clear()
        {
            var ids = _items.Keys.ToArray();
            _items.Clear();
            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        public bool Contains(TCloudItem item) => _items.ContainsKey(item.Id);

        public void CopyTo(TCloudItem[] array, int arrayIndex)
        {
            _items.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TCloudItem item)
        {
            var res = _items.Remove(item.Id);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new []{item.Id}));
            return res;
        }

        public int Count => _items.Count;

        [Obsolete("Useless property. Exists only as IList implementation.")]
        public bool IsReadOnly => false;

        [Obsolete("Useless method. Exists only as IList implementation.")]
        public int IndexOf(TCloudItem item) => item.Id;

        [Obsolete("Useless method. Exists only as IList implementation.")]
        public void Insert(int index, TCloudItem item) => Add(item);

        public void RemoveAt(int index)
        {
            _items.Remove(index);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new []{index}));
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
            foreach (var ci in items)
            {
                _items.Add(ci.Id, ci);
            }
            OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(items));
        }

        public void Remove(IEnumerable<TCloudItem> items)
        {
            foreach (var ci in items)
            {
                _items.Remove(ci.Id);
            }
            OnRemoved?.Invoke(this, new RemovedEventArgs(items.Select(p => p.Id)));
        }
        
        public void UpdateItem(TCloudItem item)
        {
            _items[item.Id] = item;
            OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(new []{item}));
        }

        public void UpdateItems(IEnumerable<TCloudItem> items)
        {
            foreach (var ci in items)
            {
                _items[ci.Id] = ci;
            }
            OnUpdated?.Invoke(this, new UpdatedEventArgs<TCloudItem>(items));
        }

        #endregion

        #region Private definitions

        private readonly SortedDictionary<int, TCloudItem> _items = new SortedDictionary<int, TCloudItem>();

        #endregion
    }
}