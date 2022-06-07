﻿using System.Collections.Generic;
using System.Linq;
using Elektronik.DataObjects;

namespace Elektronik.DataSources.Containers.EventArgs
{
    /// <summary> Event args for adding cloud items. </summary>
    /// <typeparam name="T"> Type of cloud items. </typeparam>
    public class AddedEventArgs<T> : System.EventArgs
            where T : struct, ICloudItem
    {
        public readonly IList<T> AddedItems;

        public AddedEventArgs(IList<T> addedItems)
        {
            AddedItems = addedItems;
        }

        public AddedEventArgs(T item)
        {
            AddedItems = new[] { item };
        }

        protected bool Equals(AddedEventArgs<T> other)
        {
            if (ReferenceEquals(AddedItems, other.AddedItems)) return true;
            if (AddedItems.Count != other.AddedItems.Count) return false;
            foreach (var (first, second) in AddedItems.Zip(other.AddedItems, (arg1, arg2) => (arg1, arg2)))
            {
                if (!Equals(first, second)) return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddedEventArgs<T>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (AddedItems != null ? AddedItems.GetHashCode() : 0);
        }
    }
}