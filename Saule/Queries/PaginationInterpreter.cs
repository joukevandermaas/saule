using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Saule.Queries
{
    internal class PaginationInterpreter
    {
        public static object ApplyPaginationIfApplicable(PaginationContext context, object data)
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

        public PaginationInterpreter(PaginationContext context)
        {
            Context = context;
        }

        public PaginationContext Context { get; }

        public IQueryable Apply(IQueryable queryable)
        {
            // Skip does not work on queryables by default, because it makes
            // no sense if the order is not determined. This means we have to
            // order the queryable first, before we can apply pagination.
            var isOrdered = queryable.GetType().GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof (IOrderedQueryable<>));

            var ordered = isOrdered ? queryable : OrderById(queryable);

            var filtered = ordered.ApplyQuery(QueryMethod.Skip, Context.Page * Context.PerPage) as IQueryable;
            filtered = filtered.ApplyQuery(QueryMethod.Take, Context.PerPage) as IQueryable;

            return filtered;
        }

        private static IQueryable OrderById(IQueryable queryable)
        {
            var funcType = typeof(Func<,>).MakeGenericType(queryable.ElementType, typeof(object));
            var param = Expression.Parameter(queryable.ElementType, "i");
            var property = Expression.Property(param, "Id");

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

            var expression = expressionFactory.Invoke(null, new object[] { property, new[] { param } });

            var ordered = queryable.ApplyQuery(QueryMethod.OrderBy, expression) as IQueryable;
            return ordered;
        }

        public IEnumerable Apply(IEnumerable queryable)
        {
            var filtered = queryable.ApplyQuery(QueryMethod.Skip, Context.Page * Context.PerPage) as IEnumerable;
            filtered = filtered.ApplyQuery(QueryMethod.Take, Context.PerPage) as IEnumerable;

            return filtered;
        }

    }
}
