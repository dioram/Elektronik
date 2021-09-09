using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Commands;
using JetBrains.Annotations;

namespace Elektronik.Data
{
    public class OnlineFrameBuffer
    {
        public class Frame
        {
            public readonly DateTime Timestamp;
            public TimeSpan ToNext;
            public readonly bool IsKeyFrame;
            public readonly ICommand Command;

            public Frame(DateTime timestamp, bool isKeyFrame, ICommand command)
            {
                Timestamp = timestamp;
                ToNext = default;
                IsKeyFrame = isKeyFrame;
                Command = command;
            }
        }
        
        private readonly List<Frame> _buffer = new List<Frame>();
        private int _index = -1;

        public event Action<int> FramesAmountChanged;
        
        public void Add(ICommand item, DateTime timestamp, bool isKeyFrame)
        {
            if (_buffer.Count > 0)
            {
                _buffer.Last().ToNext = timestamp - _buffer.Last().Timestamp;
            }
            _buffer.Add(new Frame(timestamp, isKeyFrame, item));
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

        public int CurrentSize => _buffer.Count;
        
        public int CurrentIndex => _index;

        public DateTime CurrentTimeStamp => _index >= 0 ? _buffer[_index].Timestamp : DateTime.Now;

        public bool MoveNext()
        {
            if (!HasMore) return false;
            
            ++_index;
            return true;
        }

        public void Reset()
        {
            _buffer.Clear();
            _index = -1;
            FramesAmountChanged?.Invoke(0);
        }

        public bool HasMore => _index < _buffer.Count - 1;

        [CanBeNull]
        public Frame Current
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