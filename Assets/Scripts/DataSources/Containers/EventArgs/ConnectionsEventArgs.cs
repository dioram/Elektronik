using System.Collections.Generic;
using System.Linq;

namespace Elektronik.DataSources.Containers.EventArgs
{
    public class ConnectionsEventArgs : System.EventArgs
    {
        public readonly IList<(int id1, int id2)> Items;

        public ConnectionsEventArgs(IList<(int id1, int id2)> items)
        {
            Items = items;
        }

        protected bool Equals(ConnectionsEventArgs other)
        {
            if (ReferenceEquals(Items, other.Items)) return true;
            if (Items.Count() != other.Items.Count()) return false;
            foreach (var (first, second) in Items.Zip(other.Items, (arg1, arg2) => (arg1, arg2)))
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
            return Equals((ConnectionsEventArgs)obj);
        }

        public override int GetHashCode()
        {
            return (Items != null ? Items.GetHashCode() : 0);
        }
    }
}