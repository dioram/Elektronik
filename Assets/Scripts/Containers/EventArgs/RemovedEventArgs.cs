using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Containers.EventArgs
{
    public class RemovedEventArgs : System.EventArgs
    {
        public readonly IList<int> RemovedIds;

        public RemovedEventArgs(IList<int> removedIds)
        {
            RemovedIds = removedIds;
        }

        public RemovedEventArgs(int removedId)
        {
            RemovedIds = new List<int> { removedId };
        }

        protected bool Equals(RemovedEventArgs other)
        {
            if (ReferenceEquals(RemovedIds, other.RemovedIds)) return true;
            if (RemovedIds.Count != other.RemovedIds.Count) return false;
            foreach (var (first, second) in RemovedIds.Zip(other.RemovedIds, (arg1, arg2) => (arg1, arg2)))
            {
                if (!Equals(first, second)) return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RemovedEventArgs)obj);
        }

        public override int GetHashCode()
        {
            return (RemovedIds != null ? RemovedIds.GetHashCode() : 0);
        }
    }
}