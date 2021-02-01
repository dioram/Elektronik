﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Containers
{
    public interface IContainer<T> : IList<T>
    {
        T this[T obj] { get; set; }
        void Add(IEnumerable<T> objects);
        IList<T> GetAll();
        void Remove(IEnumerable<T> objs);
        bool TryGet(T obj, out T current);
        void Update(T obj);
        void Update(IEnumerable<T> objs);
    }
}