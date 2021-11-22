using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Elektronik.Extensions
{
    public static class LinqExtensions
    {
        // TODO: Remove them when unity will support .net5
        [Pure]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, 
                                                   IComparer<TKey> comparer = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer ??= Comparer<TKey>.Default;
            
            var result = source.Take(1).ToList().First();
            var maxKey = selector(result);
            
            foreach (var item in source)
            {
                var newKey = selector(item);
                if (comparer.Compare(maxKey, newKey) >= 0) continue;
                
                maxKey = newKey;
                result = item;
            }
            return result;
        }
        
        [Pure]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, 
                                                   IComparer<TKey> comparer = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer ??= Comparer<TKey>.Default;
            
            var result = source.Take(1).ToList().First();
            var minKey = selector(result);
            
            foreach (var item in source)
            {
                var newKey = selector(item);
                if (comparer.Compare(minKey, newKey) <= 0) continue;
                
                minKey = newKey;
                result = item;
            }

            return result;
        }
    }
}