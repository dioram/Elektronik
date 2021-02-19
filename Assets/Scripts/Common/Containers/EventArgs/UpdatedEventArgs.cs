using System;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public class UpdatedEventArgs<T> : EventArgs
    {
        public readonly IEnumerable<T> UpdatedItems;

        public UpdatedEventArgs(IEnumerable<T> updatedItems)
        {
            UpdatedItems = updatedItems;
        }
    }
}