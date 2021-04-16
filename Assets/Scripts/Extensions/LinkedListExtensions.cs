using System.Collections.Generic;

namespace Elektronik.Extensions
{
    public static class LinkedListExtensions
    {
        public static void MoveFrom<T>(this LinkedList<T> to, LinkedList<T> from)
        {
            while (from.First != null)
            {
                var node = from.First;
                from.Remove(node);
                to.AddLast(node);
            }
        }
    }
}
