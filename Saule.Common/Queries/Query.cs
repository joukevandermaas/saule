using System.Collections;
using System.Linq;
using Saule.Http;
using Saule.Queries.Filtering;
using Saule.Queries.Pagination;
using Saule.Queries.Sorting;

namespace Saule.Queries
{
    internal static class Query
    {
        public static object ApplySorting(object data, SortingContext context, ApiResource resource)
        {
            var queryable = data as IQueryable;
            if (queryable != null)
            {
                return new SortingInterpreter(context, resource).Apply(queryable);
            }

            var enumerable = data as IEnumerable;
            if (enumerable != null)
            {
                // all queryables are enumerable, so this needs to be after
                // the queryable case
                return new SortingInterpreter(context, resource).Apply(enumerable);
            }

            return data;
        }

        public static object ApplyPagination(object data, PaginationContext context, ApiResource resource)
        {
            var queryable = data as IQueryable;
            if (queryable != null)
            {
                return new PaginationInterpreter(context, resource).Apply(queryable);
            }

            var enumerable = data as IEnumerable;
            if (enumerable != null)
            {
                // all queryables are enumerable, so this needs to be after
                // the queryable case
                return new PaginationInterpreter(context, resource).Apply(enumerable);
            }

            return data;
        }

        public static object ApplyFiltering(object data, FilteringContext context, ApiResource resource)
        {
            var queryable = data as IQueryable;
            if (queryable != null)
            {
                return new FilteringInterpreter(context, resource).Apply(queryable);
            }

            var enumerable = data as IEnumerable;
            if (enumerable != null)
            {
                // all queryables are enumerable, so this needs to be after
                // the queryable case
                return new FilteringInterpreter(context, resource).Apply(enumerable);
            }

            return data;
        }
    }
}
