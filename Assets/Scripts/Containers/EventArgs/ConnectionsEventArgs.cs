using System.Collections.Generic;

namespace Elektronik.Containers.EventArgs
{
    public class ConnectionsEventArgs : System.EventArgs
    {
        public readonly IEnumerable<(int id1, int id2)> Items;

        public ConnectionsEventArgs(IEnumerable<(int id1, int id2)> items)
        {
            Items = items;
        }
    }
}