using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public class SlamInfinitePlanesContainer : MonoBehaviour, IContainer<SlamInfinitePlane>
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
                ItemsAdded += Renderer.OnItemsAdded;
                ItemsUpdated += Renderer.OnItemsUpdated;
                ItemsRemoved += Renderer.OnItemsRemoved;
            }
        }

        private void OnDestroy()
        {
            Clear();
        }

        #endregion

        #region IContainer implementation
        
        public event Action<IContainer<SlamInfinitePlane>, IEnumerable<SlamInfinitePlane>> ItemsAdded;
        
        public event Action<IContainer<SlamInfinitePlane>, IEnumerable<SlamInfinitePlane>> ItemsUpdated;
        
        public event Action<IContainer<SlamInfinitePlane>, IEnumerable<int>> ItemsRemoved;

        public SlamInfinitePlane this[int index]
        {
            get => _planes[index];
            set
            {
                _planes[index] = value;
                ItemsUpdated?.Invoke(this, new []{value});
            }
        }

        public SlamInfinitePlane this[SlamInfinitePlane obj]
        {
            get => _planes[obj.id];
            set
            {
                _planes[obj.id] = value;
                ItemsUpdated?.Invoke(this, new []{value});
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
            return item.id;
        }

        public bool Contains(SlamInfinitePlane item)
        {
            return _planes.ContainsKey(item.id);
        }

        public void CopyTo(SlamInfinitePlane[] array, int arrayIndex)
        {
            _planes.Values.CopyTo(array, arrayIndex);
        }

        public void Add(SlamInfinitePlane item)
        {
            _planes[item.id] = item;
            ItemsAdded?.Invoke(this, new []{item});
        }

        public void AddRange(IEnumerable<SlamInfinitePlane> objects)
        {
            foreach (var plane in objects)
            {
                _planes.Add(plane.id, plane);
            }
            ItemsAdded?.Invoke(this, objects);
        }

        public void Insert(int index, SlamInfinitePlane item)
        {
            _planes[index] = item;
            ItemsAdded?.Invoke(this, new []{item});
        }

        public void Clear()
        {
            var ids = _planes.Keys.ToArray();
            _planes.Clear();
            ItemsRemoved?.Invoke(this, ids);
        }

        public bool Remove(SlamInfinitePlane item)
        {
            var res = _planes.Remove(item.id);
            ItemsRemoved?.Invoke(this, new [] {item.id});
            return res;
        }

        public void RemoveAt(int index)
        {
            _planes.Remove(index);
            ItemsRemoved?.Invoke(this, new []{index});
        }

        public void Remove(IEnumerable<SlamInfinitePlane> objs)
        {
            foreach (var plane in objs)
            {
                _planes.Remove(plane.id);
            }
            ItemsRemoved?.Invoke(this, objs.Select(o => o.id));
        }

        public bool TryGet(SlamInfinitePlane obj, out SlamInfinitePlane current)
        {
            if (_planes.ContainsKey(obj.id))
            {
                current = _planes[obj.id];
                return true;
            }

            current = default;
            return false;
        }

        public void UpdateItem(SlamInfinitePlane obj)
        {
            _planes[obj.id] = obj;
            ItemsUpdated?.Invoke(this, new []{obj});
        }

        public void UpdateItems(IEnumerable<SlamInfinitePlane> objs)
        {
            foreach (var plane in objs)
            {
                _planes[plane.id] = plane;
            }
            ItemsUpdated?.Invoke(this, objs);
        }

        #endregion
        
        #region Private definitions
        
        private readonly Dictionary<int, SlamInfinitePlane> _planes = new Dictionary<int, SlamInfinitePlane>();
        
        #endregion
    }
}