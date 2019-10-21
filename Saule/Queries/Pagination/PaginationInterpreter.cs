using System;
using System.Collections;
using System.Linq;

namespace Saule.Queries.Pagination
{
    internal class PaginationInterpreter
    {
        private readonly PaginationContext _context;
        private readonly ApiResource _resource;

        public PaginationInterpreter(PaginationContext context, ApiResource resource)
        {
            _context = context;
            _resource = resource;
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

        private IQueryable OrderById(IQueryable queryable)
        {
            try
            {
                var sorted = queryable.ApplyQuery(
                    QueryMethod.OrderBy,
                    Lambda.SelectProperty(queryable.ElementType, _resource.IdProperty));
                return sorted as IQueryable;
            }
            catch (ArgumentException ex)
            {
                // property id not found
                throw new JsonApiException(
                    ErrorType.Server,
                    $"Type {queryable.ElementType.Name} does not have a property called '{_resource.IdProperty}'.",
                    ex);
            }
        }
    }
}
