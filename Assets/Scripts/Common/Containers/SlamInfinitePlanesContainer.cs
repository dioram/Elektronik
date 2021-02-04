using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Clouds.Meshes;
using Elektronik.Common.Data.PackageObjects;

namespace Elektronik.Common.Containers
{
    public class SlamInfinitePlanesContainer : IContainer<SlamInfinitePlane>
    {
        private Dictionary<int, SlamInfinitePlane> m_planes = new Dictionary<int, SlamInfinitePlane>();
        private IFastPlanesCloud m_planeCloud;

        public SlamInfinitePlanesContainer(IFastPlanesCloud planesCloud)
        {
            m_planeCloud = planesCloud;
        }
        
        public IEnumerator<SlamInfinitePlane> GetEnumerator()
        {
            return m_planes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(SlamInfinitePlane item)
        {
            m_planes[item.id] = item;
            var p = new CloudPlane(item.id, item.offset, item.normal, item.color);
            m_planeCloud.Set(p);
        }

        public void Clear()
        {
            m_planes.Clear();
            m_planeCloud.Clear();
        }

        public bool Contains(SlamInfinitePlane item)
        {
            return m_planes.ContainsKey(item.id);
        }

        public void CopyTo(SlamInfinitePlane[] array, int arrayIndex)
        {
            m_planes.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(SlamInfinitePlane item)
        {
            m_planeCloud.Set(CloudPlane.Empty(item.id));
            return m_planes.Remove(item.id);
        }

        public int Count => m_planes.Count;
        public bool IsReadOnly => false;
        public int IndexOf(SlamInfinitePlane item)
        {
            return item.id;
        }

        public void Insert(int index, SlamInfinitePlane item)
        {
            m_planes[index] = item;
        }

        public void RemoveAt(int index)
        {
            m_planeCloud.Set(CloudPlane.Empty(index));
            m_planes.Remove(index);
        }

        public SlamInfinitePlane this[int index]
        {
            get => m_planes[index];
            set => m_planes[index] = value;
        }

        public SlamInfinitePlane this[SlamInfinitePlane obj]
        {
            get => m_planes[obj.id];
            set => m_planes[obj.id] = value;
        }

        public void Add(IEnumerable<SlamInfinitePlane> objects)
        {
            foreach (var plane in objects)
            {
                Add(plane);
            }
        }

        public IList<SlamInfinitePlane> GetAll()
        {
            return m_planes.Values.ToList();
        }

        public void Remove(IEnumerable<SlamInfinitePlane> objs)
        {
            foreach (var plane in objs)
            {
                Remove(plane);
            }
        }

        public bool TryGet(SlamInfinitePlane obj, out SlamInfinitePlane current)
        {
            if (m_planes.ContainsKey(obj.id))
            {
                current = m_planes[obj.id];
                return true;
            }

            current = default;
            return false;
        }

        public void Update(SlamInfinitePlane obj)
        {
            var p = new CloudPlane(obj.id, obj.offset, obj.normal, obj.color);
            m_planeCloud.Set(p);
            m_planes[obj.id] = obj;
        }

        public void Update(IEnumerable<SlamInfinitePlane> objs)
        {
            foreach (var plane in objs)
            {
                Update(plane);
            }
        }
    }
}