using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Containers.EventArgs
{
    public class AddedEventArgs<T> : System.EventArgs
    {
        public readonly IEnumerable<T> AddedItems;

        public AddedEventArgs(IEnumerable<T> addedItems)
        {
            AddedItems = addedItems;
        }
        
        public AddedEventArgs(T item)
        {
            AddedItems = new []{item};
        }

        protected bool Equals(AddedEventArgs<T> other)
        {
            if (ReferenceEquals(AddedItems, other.AddedItems)) return true;
            if (AddedItems.Count() != other.AddedItems.Count()) return false;
            foreach (var (first, second) in AddedItems.Zip(other.AddedItems, (arg1, arg2) => (arg1, arg2)))
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
            return Equals((AddedEventArgs<T>)obj);
        }

        public override int GetHashCode()
        {
            return (AddedItems != null ? AddedItems.GetHashCode() : 0);
        }
    }
}