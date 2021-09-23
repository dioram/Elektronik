using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Elektronik.Plugins.Common.Parsing
{
    public static class ChainExtensions
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static T? BuildChain<T>(this IEnumerable<IChainable<T>> collection)
        {
            var first = collection.FirstOrDefault();
            var link = first;
            foreach (var nextLink in collection.Skip(1))
            {
                link = link?.SetSuccessor(nextLink);
            }
            return (T)first;
        }
    }
}
