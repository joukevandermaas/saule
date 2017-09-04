using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Queries
{
    internal sealed class QueryMethod
    {
        private readonly MethodInfo _queryable;
        private readonly MethodInfo _enumerable;
        private readonly QueryType _queryType;

        private QueryMethod(MethodInfo queryable, MethodInfo enumerable, QueryType queryType)
        {
            _queryable = queryable;
            _enumerable = enumerable;
            _queryType = queryType;
        }

        private enum QueryType
        {
            Simple,
            Func,
            Predicate
        }

        // This idea was borrowed from the OData repo. See the following url:
        // https://github.com/OData/WebApi/blob/master/OData/src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs
        public static QueryMethod Skip => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).Skip(default(int))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).Skip(default(int))),
            QueryType.Simple);

        public static QueryMethod Take => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).Take(default(int))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).Take(default(int))),
            QueryType.Simple);

        public static QueryMethod OrderBy => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).OrderBy(default(Expression<Func<int, int>>))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).OrderBy(default(Func<int, int>))),
            QueryType.Func);

        public static QueryMethod OrderByDescending => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).OrderByDescending(default(Expression<Func<int, int>>))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).OrderByDescending(default(Func<int, int>))),
            QueryType.Func);

        public static QueryMethod ThenBy => new QueryMethod(
            GetGenericMethodInfo(_ => default(IOrderedQueryable<int>).ThenBy(default(Expression<Func<int, int>>))),
            GetGenericMethodInfo(_ => default(IOrderedEnumerable<int>).ThenBy(default(Func<int, int>))),
            QueryType.Func);

        public static QueryMethod ThenByDescending => new QueryMethod(
            GetGenericMethodInfo(_ => default(IOrderedQueryable<int>).ThenByDescending(default(Expression<Func<int, int>>))),
            GetGenericMethodInfo(_ => default(IOrderedEnumerable<int>).ThenByDescending(default(Func<int, int>))),
            QueryType.Func);

        public static QueryMethod Where => new QueryMethod(
            GetGenericMethodInfo(_ => default(IQueryable<int>).Where(default(Expression<Func<int, bool>>))),
            GetGenericMethodInfo(_ => default(IEnumerable<int>).Where(default(Func<int, bool>))),
            QueryType.Predicate);

        public object ApplyTo(IQueryable queryable, params object[] arguments)
        {
            var invokeArgs = new[] { queryable }.Concat(arguments).ToArray();
            var typeArguments = GetTypeArguments(invokeArgs);
            var typed = _queryable.MakeGenericMethod(typeArguments);
            return typed.Invoke(null, invokeArgs);
        }

        public object ApplyTo(IEnumerable enumerable, params object[] arguments)
        {
            if (_queryType != QueryType.Simple)
            {
                // we need to compile the expression
                var func = (LambdaExpression)arguments[0];
                var compiled = func.Compile();
                arguments[0] = compiled;
            }

            var invokeArgs = new[] { enumerable }.Concat(arguments).ToArray();
            var typeArguments = GetTypeArguments(invokeArgs);
            var typed = _enumerable.MakeGenericMethod(typeArguments);
            return typed.Invoke(null, invokeArgs);
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

        private static Type[] GetPredicateTypeArguments(object[] arguments)
        {
            var predicateType = arguments[1].GetType();
            if (typeof(Expression).IsAssignableFrom(predicateType))
            {
                predicateType = predicateType.GenericTypeArguments[0];
            }

            return predicateType.GenericTypeArguments.Take(1).ToArray();
        }

        private static Type[] GetFuncTypeArguments(object[] arguments)
        {
            // Type params are the same as the func in the expression.
            // (arguments[1] is Expression<Func<SomeType, object>> or
            // Func<SomeType, object>)
            var type = arguments[1].GetType();
            if (typeof(Expression).IsAssignableFrom(type))
            {
                type = type.GenericTypeArguments[0];
            }

            return type.GenericTypeArguments;
        }

        private static Type[] GetSimpleTypeArguments(object[] arguments)
        {
            var enumerable = arguments[0] // IQueryable<> extends IEnumerable<>
                .GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .First(i => typeof(IEnumerable<>).IsAssignableFrom(i.GetGenericTypeDefinition()));
            return enumerable.GetGenericArguments();
        }

        private Type[] GetTypeArguments(params object[] arguments)
        {
            switch (_queryType)
            {
                case QueryType.Simple:
                    return GetSimpleTypeArguments(arguments);
                case QueryType.Func:
                    return GetFuncTypeArguments(arguments);
                case QueryType.Predicate:
                    return GetPredicateTypeArguments(arguments);
                default:
                    throw new InvalidOperationException("Unable to apply user query.");
            }
        }
    }
}
