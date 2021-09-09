using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Elektronik.Offline
{
    public class FramesCollection<T> : IDisposable
            where T : class
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly List<T> _buffer;
        private int _index;
        private readonly int _framesAmount;

        public FramesCollection(Func<int, IEnumerator<T>> enumerator, int framesAmount = 0) : this(framesAmount)
        {
            _enumerator = enumerator?.Invoke(framesAmount) ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public FramesCollection(Func<IEnumerator<T>> enumerator, int framesAmount = 0) : this(framesAmount)
        {
            _enumerator = enumerator?.Invoke() ?? throw new ArgumentNullException(nameof(enumerator));
        }

        private FramesCollection(int framesAmount)
        {
            _framesAmount = framesAmount;
            _buffer = new List<T>();
            if (framesAmount > 0) _buffer.Capacity = framesAmount;
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

        public int CurrentSize => _framesAmount > 0 ? _framesAmount : _buffer.Count;
        
        public int CurrentIndex => _index;

        public event Action<int> OnSizeChanged;

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
                if (_framesAmount == 0) OnSizeChanged?.Invoke(_buffer.Count);
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

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}