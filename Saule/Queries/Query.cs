using System.Collections;
using System.Linq;
using Saule.Queries.Filtering;
using Saule.Queries.Pagination;
using Saule.Queries.Sorting;

namespace Saule.Queries
{
    internal static class Query
    {
        public static object ApplySorting(object data, SortingContext context)
        {
            var queryable = data as IQueryable;
            if (queryable != null)
            {
                return new SortingInterpreter(context).Apply(queryable);
            }

            var enumerable = data as IEnumerable;
            if (enumerable != null)
            {
                // all queryables are enumerable, so this needs to be after
                // the queryable case
                return new SortingInterpreter(context).Apply(enumerable);
            }

            return data;
        }

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

        public static object ApplyFiltering(object data, FilteringContext context)
        {
            var queryable = data as IQueryable;
            if (queryable != null)
            {
                return new FilteringInterpreter(context).Apply(queryable);
            }

            var enumerable = data as IEnumerable;
            if (enumerable != null)
            {
                // all queryables are enumerable, so this needs to be after
                // the queryable case
                return new FilteringInterpreter(context).Apply(enumerable);
            }

            return data;
        }
    }
}
