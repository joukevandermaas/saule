using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Queries
{
    internal class QueryMethod
    {
        private readonly MethodInfo _enumerable;
        private readonly MethodInfo _queryable;

        private QueryMethod(MethodInfo queryable, MethodInfo enumerable)
        {
            _queryable = queryable;
            _enumerable = enumerable;
        }

        // This idea was borrowed from the OData repo. See the following url:
        // https://github.com/OData/WebApi/blob/master/OData/src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        public static QueryMethod Skip => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).Skip(default(int))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).Skip(default(int))));

        public static QueryMethod Take => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).Take(default(int))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).Take(default(int))));

        public static QueryMethod OrderBy => new OrderByQueryMethod();

        public object ApplyTo(IQueryable queryable, params object[] arguments)
        {
            return ApplyToInternal(_queryable, new[] { queryable }.Concat(arguments).ToArray());
        }

        public object ApplyTo(IEnumerable enumerable, params object[] arguments)
        {
            return ApplyToInternal(_enumerable, new[] { enumerable }.Concat(arguments).ToArray());
        }

        protected static Type[] GetTypeArguments(object o)
        {
            var enumerable = o // IQueryable<> extends IEnumerable<>
                .GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .First(i => typeof(IEnumerable<>).IsAssignableFrom(i.GetGenericTypeDefinition()));
            return enumerable.GetGenericArguments();
        }

        protected virtual object ApplyToInternal(MethodInfo method, object[] arguments)
        {
            var typeArguments = GetTypeArguments(arguments[0]);
            var typed = method.MakeGenericMethod(typeArguments);
            return typed.Invoke(null, arguments);
        }

        private static MethodInfo GetGenericMethodInfo<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            return GetGenericMethodInfo(expression as Expression);
        }

        private static MethodInfo GetGenericMethodInfo(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;

            return (lambdaExpression?.Body as MethodCallExpression)?.Method.GetGenericMethodDefinition();
        }

        private class OrderByQueryMethod : QueryMethod
        {
            public OrderByQueryMethod()
                : base(
                GetGenericMethodInfo(_ => default(IQueryable<int>).OrderBy(default(Expression<Func<int, int>>))),
                GetGenericMethodInfo(_ => default(IQueryable<int>).OrderBy(default(Expression<Func<int, int>>))))
            {
            }

            protected override object ApplyToInternal(MethodInfo method, object[] arguments)
            {
                // Type params are the same as the func in the expression.
                // (arguments[0] is Expression<Func<SomeType, object>>)
                var typeParams = arguments[1].GetType().GenericTypeArguments[0].GenericTypeArguments;
                var typed = method.MakeGenericMethod(typeParams);

                return typed.Invoke(null, arguments);
            }
        }
    }
}
