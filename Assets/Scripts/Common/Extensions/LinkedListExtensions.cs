using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Extensions
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
