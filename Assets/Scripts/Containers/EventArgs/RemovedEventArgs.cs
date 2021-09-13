using System.Collections.Generic;

namespace Elektronik.Containers.EventArgs
{
    public class RemovedEventArgs : System.EventArgs
    {
        public readonly IList<int> RemovedIds;

        public RemovedEventArgs(IList<int> removedIds)
        {
            RemovedIds = removedIds;
        }
    }
}