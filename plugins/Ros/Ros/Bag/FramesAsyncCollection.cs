using System;
using System.Collections.Generic;

namespace Elektronik.RosPlugin.Ros.Bag
{
    public class FramesAsyncCollection<T> : IDisposable
            where T : class
    {
        public event Action<int>? OnCurrentSizeChanged;
        
        public FramesAsyncCollection(Func<IAsyncEnumerator<T>> enumerator, int framesAmount = 0)
        {
            _enumeratorFunc = enumerator;
            _isSizeKnown = framesAmount != 0;
            _framesAmount = framesAmount;
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

        public int CurrentSize => _isSizeKnown ? _framesAmount : _buffer.Count;
        
        public int CurrentIndex => _index;

        public bool MoveNext()
        {
            _enumerator ??= _enumeratorFunc.Invoke();
            if (_index < _buffer.Count - 1)
            {
                ++_index;
                return true;
            }

            if (_enumerator.MoveNextAsync().Result)
            {
                _buffer.Add(_enumerator.Current);
                if (!_isSizeKnown) OnCurrentSizeChanged?.Invoke(_buffer.Count);
                ++_index;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _enumerator = _enumeratorFunc.Invoke();
            _buffer.Clear();
            if (!_isSizeKnown) OnCurrentSizeChanged?.Invoke(_buffer.Count);
            _index = -1;
        }

        public T? Current
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
            _enumerator?.DisposeAsync();
        }

        #region Private

        private IAsyncEnumerator<T>? _enumerator;
        private readonly List<T> _buffer;
        private int _index;
        private readonly bool _isSizeKnown;
        private readonly int _framesAmount;
        private readonly Func<IAsyncEnumerator<T>> _enumeratorFunc;

        #endregion
    }
}