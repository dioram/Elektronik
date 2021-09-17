using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;

namespace Elektronik.DataSources.Containers.EventArgs
{
    public class RemovedEventArgs<T> : System.EventArgs
            where T : struct, ICloudItem
    {
        public readonly IList<T> RemovedItems;

        public RemovedEventArgs(IList<T> removedItems)
        {
            RemovedItems = removedItems;
        }

        public RemovedEventArgs(T removedItem)
        {
            RemovedItems = new List<T> { removedItem };
        }

        public RemovedEventArgs(IList<int> removedIds)
        {
            RemovedItems = removedIds.Select(i => new T { Id = i, Message = "" }).ToArray();
        }

        protected bool Equals(RemovedEventArgs<T> other)
        {
            if (ReferenceEquals(RemovedItems, other.RemovedItems)) return true;
            if (RemovedItems.Count != other.RemovedItems.Count) return false;
            foreach (var (first, second) in RemovedItems.Zip(other.RemovedItems, (arg1, arg2) => (arg1.Id, arg2.Id)))
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
            return Equals((RemovedEventArgs<T>)obj);
        }

        public override int GetHashCode()
        {
            return (RemovedItems != null ? RemovedItems.GetHashCode() : 0);
        }
    }
}