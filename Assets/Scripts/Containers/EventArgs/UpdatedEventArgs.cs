using System.Collections.Generic;

namespace Elektronik.Containers.EventArgs
{
    public class UpdatedEventArgs<T> : System.EventArgs
    {
        public readonly IEnumerable<T> UpdatedItems;

        public UpdatedEventArgs(IEnumerable<T> updatedItems)
        {
            UpdatedItems = updatedItems;
        }
    }
}