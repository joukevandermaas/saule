using System.Collections;
using System.Linq;

namespace Saule.Queries
{
    internal static class QueryableExtensions
    {
        public static object ApplyQuery(this IQueryable queryable, QueryMethod method, params object[] arguments)
        {
            return method.ApplyTo(queryable, arguments);
        }

        public static object ApplyQuery(this IEnumerable enumerable, QueryMethod method, params object[] arguments)
        {
            return method.ApplyTo(enumerable, arguments);
        }
    }
}