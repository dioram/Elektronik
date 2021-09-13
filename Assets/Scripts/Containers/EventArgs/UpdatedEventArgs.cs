using System.Collections.Generic;
using System.Linq;
using Elektronik.Data.PackageObjects;

namespace Elektronik.Containers.EventArgs
{
    public class UpdatedEventArgs<T> : System.EventArgs
            where T : struct, ICloudItem
    {
        public readonly IList<T> UpdatedItems;

        public UpdatedEventArgs(IList<T> updatedItems)
        {
            UpdatedItems = updatedItems;
        }

        public UpdatedEventArgs(T updatedItem)
        {
            UpdatedItems = new []{updatedItem};
        }

        protected bool Equals(UpdatedEventArgs<T> other)
        {
            if (ReferenceEquals(UpdatedItems, other.UpdatedItems)) return true;
            if (UpdatedItems.Count() != other.UpdatedItems.Count()) return false;
            foreach (var (first, second) in UpdatedItems.Zip(other.UpdatedItems, (arg1, arg2) => (arg1, arg2)))
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
            return Equals((UpdatedEventArgs<T>)obj);
        }

        public override int GetHashCode()
        {
            return (UpdatedItems != null ? UpdatedItems.GetHashCode() : 0);
        }
    }
}