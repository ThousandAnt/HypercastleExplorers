using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hypercastle.Web
{
    public static class CollectionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FirstIndex<T>(this IList<T> collection, Func<T, bool> condition) where T : IEquatable<T>
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (condition.Invoke(collection[i]))
                    return i;
            }
            return -1;
        }
    }
}
