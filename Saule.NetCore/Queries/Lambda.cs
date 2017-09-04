using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Saule.Http;

namespace Saule.Queries
{
    internal static class Lambda
    {
        public static Expression SelectPropertyValue(Type type, string property, string value, QueryFilterExpressionCollection queryFilter)
        {
            var valueType = GetPropertyType(type, property);
            var parsedValue = TryConvert(value, valueType);
            var param = Expression.Parameter(type, "i");
            var propertyExpression = Expression.Property(param, property);

            var expression = queryFilter.GetQueryFilterExpression(type.GetProperty(property));

            return typeof(Lambda)
                .GetMethod(nameof(Convert), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(valueType, type)
                .Invoke(null, new[] { expression, parsedValue, propertyExpression, param })
                as Expression;
        }

        public static Expression SelectProperty(Type type, string property)
        {
            var returnType = GetPropertyType(type, property);
            var funcType = typeof(Func<,>).MakeGenericType(type, returnType);
            var param = Expression.Parameter(type, "i");
            var propertyExpression = Expression.Property(param, property);

            var expressionFactory = CreateExpressionFactory(funcType);

            return expressionFactory.Invoke(null, new object[] { propertyExpression, new[] { param } }) as Expression;
        }

        // Return value is used through reflection invocation
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static Expression<Func<TClass, bool>> Convert<TProperty, TClass>(
            Expression<Func<TProperty, TProperty, bool>> expression,
            TProperty constant,
            MemberExpression propertyExpression,
            ParameterExpression parameter)
        {
            var curriedBody = new FilterLambdaVisitor<TProperty>(propertyExpression, constant).Visit(expression.Body);
            return Expression.Lambda<Func<TClass, bool>>(curriedBody, parameter);
        }

        private static object TryConvert(string value, Type type)
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromInvariantString(value);
        }

        private static Type GetPropertyType(Type type, string property)
        {
            var returnType = type.GetProperty(property)?.PropertyType;
            if (returnType == null)
            {
                throw new ArgumentException(
                    $"Property {property} does not exist.",
                    nameof(property));
            }

            return returnType;
        }

        private static MethodInfo CreateExpressionFactory(Type funcType)
        {
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
            return expressionFactory;
        }

        private class FilterLambdaVisitor<T> : ExpressionVisitor
        {
            private readonly MemberExpression _property;
            private readonly T _constant;

            private int _parameterCounter = 0;

            public FilterLambdaVisitor(MemberExpression property, T constant)
            {
                _property = property;
                _constant = constant;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                // user's `(left, right) => left == right` should
                // turn into `(i) => i.Property == "constant"`
                if (_parameterCounter++ == 0)
                {
                    return _property;
                }

                return Expression.Constant(_constant, typeof(T));
            }
        }
    }
}
