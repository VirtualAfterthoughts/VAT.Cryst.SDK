using System.Collections.Generic;

namespace VAT.Shared.Extensions {
    /// <summary>
    /// Extension methods for ILists.
    /// </summary>
    public static partial class IListExtensions {
        /// <summary>
        /// Tries to add the element to the list. If it already exists, false is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool TryAdd<T>(this IList<T> list, T item) {
            if (list.Contains(item)) return false;
            list.Add(item); return true;
        }
    }
}