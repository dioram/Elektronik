using System.Collections.Generic;
using System.Linq;

namespace Elektronik.DataSources.Containers.EventArgs
{
    /// <summary> Event args for updating connections between cloud items. </summary>
    public class ConnectionsEventArgs : System.EventArgs
    {
        public readonly IList<(int id1, int id2)> Connections;

        public ConnectionsEventArgs(IList<(int id1, int id2)> connections)
        {
            Connections = connections;
        }

        protected bool Equals(ConnectionsEventArgs other)
        {
            if (ReferenceEquals(Connections, other.Connections)) return true;
            if (Connections.Count != other.Connections.Count) return false;
            foreach (var (first, second) in Connections.Zip(other.Connections, (arg1, arg2) => (arg1, arg2)))
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
            return (Connections != null ? Connections.GetHashCode() : 0);
        }
    }
}