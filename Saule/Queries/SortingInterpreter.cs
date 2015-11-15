using System.Collections;
using System.Linq;
using Saule.Queries.Sorting;

namespace Saule.Queries
{
    internal class SortingInterpreter
    {
        private readonly SortingContext _context;

        public SortingInterpreter(SortingContext context)
        {
            _context = context;
        }

        public IQueryable Apply(IQueryable queryable)
        {
            if (!_context.Properties.Any())
            {
                return queryable;
            }

            var list = _context.Properties.ToList();
            queryable = ApplyProperty(queryable, list[0], true);

            for (var i = 1; i < list.Count; i++)
            {
                queryable = ApplyProperty(queryable, list[i], false);
            }

            return queryable;
        }

        public IEnumerable Apply(IEnumerable enumerable)
        {
            if (!_context.Properties.Any())
            {
                return enumerable;
            }

            var list = _context.Properties.ToList();
            enumerable = ApplyProperty(enumerable, list[0], true);

            for (var i = 1; i < list.Count; i++)
            {
                enumerable = ApplyProperty(enumerable, list[i], false);
            }

            return enumerable;
        }

        private static IQueryable ApplyProperty(IQueryable queryable, SortingProperty property, bool isFirst)
        {
            queryable = queryable.ApplyQuery(
                GetQueryMethod(property.Direction, isFirst),
                Lambda.SelectProperty(queryable.ElementType, property.Name))
                as IQueryable;
            return queryable;
        }

        private static IEnumerable ApplyProperty(IEnumerable enumerable, SortingProperty property, bool isFirst)
        {
            var elementType = enumerable.GetType().GetGenericArguments().First();

            enumerable = enumerable.ApplyQuery(
                GetQueryMethod(property.Direction, isFirst),
                Lambda.SelectProperty(elementType, property.Name))
                as IEnumerable;
            return enumerable;
        }

        private static QueryMethod GetQueryMethod(SortingDirection direction, bool isFirst)
        {
            if (isFirst)
            {
                return direction == SortingDirection.Descending
                    ? QueryMethod.OrderByDescending
                    : QueryMethod.OrderBy;
            }

            return direction == SortingDirection.Descending
                ? QueryMethod.ThenByDescending
                : QueryMethod.ThenBy;
        }
    }
}