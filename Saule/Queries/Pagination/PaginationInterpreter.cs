using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Saule.Queries.Sorting;

namespace Saule.Queries.Pagination
{
    internal class PaginationInterpreter
    {
        private readonly PaginationContext _context;

        public PaginationInterpreter(PaginationContext context)
        {
            _context = context;
        }

        public IQueryable Apply(IQueryable queryable)
        {
            // Skip does not work on queryables by default, because it makes
            // no sense if the order is not determined. This means we have to
            // order the queryable first, before we can apply pagination.
            var isOrdered = queryable.GetType().GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IOrderedQueryable<>));

            var ordered = isOrdered ? queryable : OrderById(queryable);

            var filtered = ordered.ApplyQuery(QueryMethod.Skip, _context.Page * _context.PerPage) as IQueryable;
            filtered = filtered.ApplyQuery(QueryMethod.Take, _context.PerPage) as IQueryable;

            return filtered;
        }

        public IEnumerable Apply(IEnumerable queryable)
        {
            var filtered = queryable.ApplyQuery(QueryMethod.Skip, _context.Page * _context.PerPage) as IEnumerable;
            filtered = filtered.ApplyQuery(QueryMethod.Take, _context.PerPage) as IEnumerable;

            return filtered;
        }

        private static IQueryable OrderById(IQueryable queryable)
        {
            var sorting = new SortingContext(new[]
            {
                new KeyValuePair<string, string>(Constants.SortingQueryName, "id")
            });

            return Query.ApplySorting(queryable, sorting) as IQueryable;
        }
    }
}
