using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public interface ISlamContainer<T>
    {
        int Add(T obj);
        void AddRange(T[] objects);
        void ChangeColor(T obj);
        void Clear();
        bool Exists(T obj);
        T Get(int id);
        T Get(T obj);
        T[] GetAll();
        void Remove(int id);
        void Remove(T obj);
        void Repaint();
        void Set(T obj);
        bool TryGet(T obj, out T current);
        void Update(T obj);
        
    }
}
