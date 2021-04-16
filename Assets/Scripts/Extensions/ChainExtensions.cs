using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Extensions
{
    public static class ChainExtensions
    {
        public static T BuildChain<T>(this IEnumerable<IChainable<T>> collection)
        {
            IChainable<T> first = collection.First();
            IChainable<T> link = first;
            foreach (var nextlink in collection.Skip(1))
            {
                link = link.SetSuccessor(nextlink);
            }
            return (T)first;
        }
    }
}
