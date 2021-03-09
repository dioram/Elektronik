using System.Collections.Generic;

namespace Elektronik.Containers.EventArgs
{
    public class AddedEventArgs<T> : System.EventArgs
    {
        public readonly IEnumerable<T> AddedItems;

        public AddedEventArgs(IEnumerable<T> addedItems)
        {
            AddedItems = addedItems;
        }
    }
}