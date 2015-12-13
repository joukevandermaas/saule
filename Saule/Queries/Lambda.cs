using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Saule.Queries
{
    internal static class Lambda
    {
        public static Expression SelectPropertyValue(Type type, string property, string value)
        {
            var valueType = GetPropertyType(type, property);
            var parsedValue = TryConvert(value, valueType);
            var funcType = typeof(Func<,>).MakeGenericType(type, typeof(bool));
            var param = Expression.Parameter(type, "i");
            var propertyExpression = Expression.Property(param, property);
            var equalsExpression = Expression.Equal(
                propertyExpression,
                Expression.Constant(parsedValue));

            var expressionFactory = CreateExpressionFactory(funcType);

            return expressionFactory.Invoke(null, new object[] { equalsExpression, new[] { param } }) as Expression;
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
    }
}
