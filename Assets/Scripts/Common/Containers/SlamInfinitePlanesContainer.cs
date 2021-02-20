using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamInfinitePlanesContainer : MonoBehaviour, IClearable, IContainer<SlamInfinitePlane>
    {
        public InfinitePlaneCloudRenderer Renderer;
        
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
            OnAdded?.Invoke(this, new AddedEventArgs<SlamInfinitePlane>(this));
        }

        private void OnDisable()
        {
            OnRemoved?.Invoke(this, new RemovedEventArgs(_planes.Keys));
        }

        private void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region IContainer implementation
        
        public event Action<IContainer<SlamInfinitePlane>, AddedEventArgs<SlamInfinitePlane>> OnAdded;
        
        public event Action<IContainer<SlamInfinitePlane>, UpdatedEventArgs<SlamInfinitePlane>> OnUpdated;
        
        public event Action<IContainer<SlamInfinitePlane>, RemovedEventArgs> OnRemoved;

        public SlamInfinitePlane this[int index]
        {
            get => _planes[index];
            set
            {
                _planes[index] = value;
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamInfinitePlane>(new []{value}));
            }
        }

        public SlamInfinitePlane this[SlamInfinitePlane obj]
        {
            get => _planes[obj.Id];
            set
            {
                _planes[obj.Id] = value;
                OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamInfinitePlane>(new []{value}));
            }
        }

        public IEnumerator<SlamInfinitePlane> GetEnumerator()
        {
            return _planes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _planes.Count;
        
        public bool IsReadOnly => false;
        
        public int IndexOf(SlamInfinitePlane item)
        {
            return item.Id;
        }

        public bool Contains(SlamInfinitePlane item)
        {
            return _planes.ContainsKey(item.Id);
        }

        public void CopyTo(SlamInfinitePlane[] array, int arrayIndex)
        {
            _planes.Values.CopyTo(array, arrayIndex);
        }

        public void Add(SlamInfinitePlane item)
        {
            _planes[item.Id] = item;
            OnAdded?.Invoke(this, new AddedEventArgs<SlamInfinitePlane>(new []{item}));
        }

        public void AddRange(IEnumerable<SlamInfinitePlane> objects)
        {
            foreach (var plane in objects)
            {
                _planes.Add(plane.Id, plane);
            }
            OnAdded?.Invoke(this, new AddedEventArgs<SlamInfinitePlane>(objects));
        }

        public void Insert(int index, SlamInfinitePlane item)
        {
            _planes[index] = item;
            OnAdded?.Invoke(this, new AddedEventArgs<SlamInfinitePlane>(new []{item}));
        }

        public void Clear()
        {
            var ids = _planes.Keys.ToArray();
            _planes.Clear();
            OnRemoved?.Invoke(this, new RemovedEventArgs(ids));
        }

        public bool Remove(SlamInfinitePlane item)
        {
            var res = _planes.Remove(item.Id);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new [] {item.Id}));
            return res;
        }

        public void RemoveAt(int index)
        {
            _planes.Remove(index);
            OnRemoved?.Invoke(this, new RemovedEventArgs(new []{index}));
        }

        public void Remove(IEnumerable<SlamInfinitePlane> objs)
        {
            foreach (var plane in objs)
            {
                _planes.Remove(plane.Id);
            }
            OnRemoved?.Invoke(this, new RemovedEventArgs(objs.Select(o => o.Id)));
        }

        public bool TryGet(SlamInfinitePlane obj, out SlamInfinitePlane current)
        {
            if (_planes.ContainsKey(obj.Id))
            {
                current = _planes[obj.Id];
                return true;
            }

            current = default;
            return false;
        }

        public void UpdateItem(SlamInfinitePlane obj)
        {
            _planes[obj.Id] = obj;
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamInfinitePlane>(new []{obj}));
        }

        public void UpdateItems(IEnumerable<SlamInfinitePlane> objs)
        {
            foreach (var plane in objs)
            {
                _planes[plane.Id] = plane;
            }
            OnUpdated?.Invoke(this, new UpdatedEventArgs<SlamInfinitePlane>(objs));
        }

        #endregion
        
        #region Private definitions
        
        private readonly Dictionary<int, SlamInfinitePlane> _planes = new Dictionary<int, SlamInfinitePlane>();
        
        #endregion
    }
}