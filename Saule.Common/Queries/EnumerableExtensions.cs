using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries
{
    internal static class EnumerableExtensions
    {
        public static object ApplyQuery(this IQueryable queryable, QueryMethod method, params object[] arguments)
        {
            return method.ApplyTo(queryable, arguments);
        }

        public static object ApplyQuery(this IEnumerable enumerable, QueryMethod method, params object[] arguments)
        {
            return method.ApplyTo(enumerable, arguments);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            yield return element;
        }
    }
}