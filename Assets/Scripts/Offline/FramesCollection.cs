using System;
using System.Collections.Generic;

namespace Elektronik.ProtobufPlugin.Offline
{
    public class FramesCollection<T> : IDisposable
        where T: class
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly List<T> _buffer;
        private int _index;
        
        public FramesCollection(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _buffer = new List<T>();
            _index = -1;
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
            if (_index < _buffer.Count - 1)
            {
                ++_index;
                return true;
            }

            if (_enumerator.MoveNext())
            {
                _buffer.Add(_enumerator.Current);
                ++_index;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
            _buffer.Clear();
            _index = -1;
        }

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

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}