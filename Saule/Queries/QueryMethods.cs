using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Queries
{
    internal static class QueryMethods
    {
        public static object ApplyQuery(this IQueryable queryable, MethodInfo method, params object[] arguments)
        {
            var typed = method.MakeGenericMethod(queryable.GetType().GenericTypeArguments);

            return typed.Invoke(null, new object[] { queryable }.Concat(arguments).ToArray());
        }

        // This idea was borrowed from the OData repo. See the following url:
        // https://github.com/OData/WebApi/blob/master/OData/src/System.Web.Http.OData/OData/ExpressionHelperMethods.cs 

        public static MethodInfo Skip => GetGenericMethodInfo(_ => default(IQueryable<int>).Skip(default(int)));
        public static MethodInfo Take => GetGenericMethodInfo(_ => default(IQueryable<int>).Take(default(int)));

        private static MethodInfo GetGenericMethodInfo<TReturn>(Expression<Func<object, TReturn>> expression)
        {
            return GetGenericMethodInfo(expression as Expression);
        }
        private static MethodInfo GetGenericMethodInfo(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;

            return (lambdaExpression?.Body as MethodCallExpression)?.Method.GetGenericMethodDefinition();
        }
    }
}
