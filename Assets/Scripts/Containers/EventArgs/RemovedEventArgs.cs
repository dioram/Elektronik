using System.Collections.Generic;

namespace Elektronik.Containers.EventArgs
{
    public class RemovedEventArgs : System.EventArgs
    {
        public readonly IEnumerable<int> RemovedIds;

        public RemovedEventArgs(IEnumerable<int> removedIds)
        {
            RemovedIds = removedIds;
        }
    }
}