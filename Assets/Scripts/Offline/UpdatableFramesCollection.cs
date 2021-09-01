using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Elektronik.Offline
{
    public class UpdatableFramesCollection<T>
        where T: class
    {
        private readonly List<T> _buffer = new List<T>();
        private int _index = -1;

        public event Action<int> FramesAmountChanged;
        
        public void Add(T item)
        {
            _buffer.Add(item);
            FramesAmountChanged?.Invoke(_buffer.Count);
        }
        
        public bool MovePrevious()
        {
            if (_index <= 0)
            {
                return false;
            }

            --_index;
            return true;
        }

        public void SoftReset()
        {
            _index = -1;
        }

        public int CurrentSize => _buffer.Count;
        
        public int CurrentIndex => _index;

        public bool MoveNext()
        {
            if (_index >= _buffer.Count - 1) return false;
            
            ++_index;
            return true;
        }

        public void Reset()
        {
            _buffer.Clear();
            _index = -1;
        }

        [CanBeNull]
        public T Current
        {
            get
            {
                if (_index < 0 || _index >= _buffer.Count)
                {
                    return null;
                }

                return _buffer[_index];
            }
        }
    }
}