using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
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
            return null;
        }

        private static IQueryable ApplyProperty(IQueryable queryable, SortingProperty property, bool isFirst)
        {
            queryable = queryable.ApplyQuery(
                GetQueryMethod(property.Direction, isFirst),
                CreatePropertySelector(queryable, property.Name.ToPascalCase()))
                as IQueryable;
            return queryable;
        }

        private static object CreatePropertySelector(IQueryable queryable, string propertyName)
        {
            var returnType = queryable.ElementType.GetProperty(propertyName).PropertyType;
            var funcType = typeof(Func<,>).MakeGenericType(queryable.ElementType, returnType);
            var param = Expression.Parameter(queryable.ElementType, "i");
            var property = Expression.Property(param, propertyName);

            var expressionFactory = typeof(Expression).GetMethods()
                .Where(m => m.Name == "Lambda")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2 && x.Args.Length == 1)
                .Select(x => x.Method)
                .First()
                .MakeGenericMethod(funcType);

            return expressionFactory.Invoke(null, new object[] { property, new[] { param } });
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