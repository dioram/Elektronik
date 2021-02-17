using System;
using System.Collections.Generic;

namespace Elektronik.Common.Containers
{
    public class AddedEventArgs<T> : EventArgs
    {
        public readonly IEnumerable<T> AddedItems;

        public AddedEventArgs(IEnumerable<T> addedItems)
        {
            AddedItems = addedItems;
        }
    }
}