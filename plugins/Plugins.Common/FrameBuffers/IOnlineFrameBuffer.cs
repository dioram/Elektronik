using System;
using Elektronik.Plugins.Common.Commands;

namespace Elektronik.Plugins.Common.FrameBuffers
{
    public interface IOnlineFrameBuffer
    {
        void Add(ICommand item, DateTime timestamp, bool isKeyFrame);
    }
}