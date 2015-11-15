using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saule.Queries
{
    internal static class Query
    {
        public static object ApplyPagination(object data, PaginationContext context)
        {
            var queryable = data as IQueryable;
            if (queryable != null)
            {
                return new PaginationInterpreter(context).Apply(queryable);
            }

            var enumerable = data as IEnumerable;
            if (enumerable != null)
            {
                // all queryables are enumerable, so this needs to be after
                // the queryable case
                return new PaginationInterpreter(context).Apply(enumerable);
            }

            return data;
        }
    }
}
