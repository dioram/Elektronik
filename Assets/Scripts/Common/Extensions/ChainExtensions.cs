using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elektronik.Common.Extensions
{
    public static class ChainExtensions
    {
        public static T BuildChain<T>(this ICollection<IChainable<T>> collection)
        {
            IChainable<T> link = collection.First();
            foreach (var nextlink in collection.Skip(1))
            {
                link = link.SetSuccessor(nextlink);
            }
            return (T)collection.First();
        }
    }
}
