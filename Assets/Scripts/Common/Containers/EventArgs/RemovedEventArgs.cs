using System;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public class RemovedEventArgs : EventArgs
    {
        public readonly IEnumerable<int> RemovedIds;

        public RemovedEventArgs(IEnumerable<int> removedIds)
        {
            RemovedIds = removedIds;
        }
    }
}