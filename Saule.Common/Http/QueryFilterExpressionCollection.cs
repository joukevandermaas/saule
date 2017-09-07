using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Saule.Http.FilterExpressions;
using Saule.Queries.Filtering;

namespace Saule.Http
{
    /// <summary>
    /// Collection for implementations of <see cref="IQueryFilterExpression{T}"/>.
    /// </summary>
    public class QueryFilterExpressionCollection
    {
        private readonly Dictionary<Type, GenericDispatcher> _filters = new Dictionary<Type, GenericDispatcher>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFilterExpressionCollection"/> class.
        /// </summary>
        public QueryFilterExpressionCollection()
        {
            SetExpression(new DefaultQueryFilterExpression<object>());
        }

        /// <summary>
        /// Sets the query filter expression for properties of the specified type.
        /// </summary>
        /// <param name="queryFilter">The query filter to use.</param>
        /// <typeparam name="T">The type of the properties to use this expression for.</typeparam>
        /// <returns>The query filter expression that is registered.</returns>
        public IQueryFilterExpression<T> SetExpression<T>(IQueryFilterExpression<T> queryFilter)
        {
            var dispatcher = GenericDispatcher.GetFor(queryFilter);
            var type = typeof(T);

            if (_filters.ContainsKey(type))
            {
                _filters[type] = dispatcher;
            }
            else
            {
                _filters.Add(type, dispatcher);
            }

            return queryFilter;
        }

        /// <summary>
        /// Sets the query filter expression for properties of the specified type.
        /// </summary>
        /// <param name="expression">The query filter expression to use.</param>
        /// <typeparam name="T">The type of the properties to use this expression for.</typeparam>
        /// <returns>The query filter expression that is registered.</returns>
        public IQueryFilterExpression<T> SetExpression<T>(Expression<Func<T, T, bool>> expression)
        {
            return SetExpression(new LambdaQueryFilterExpression<T>(expression));
        }

        internal Expression GetQueryFilterExpression(PropertyInfo property)
        {
            var type = property.PropertyType;

            var candidates = type.GetInheritanceChain();
            var bestType = candidates.FirstOrDefault(c => _filters.ContainsKey(c));

            return bestType != null ?
                _filters[bestType].CallGetFilterExpression(property)
                : null;
        }

        private class GenericDispatcher
        {
            private readonly object _filter;

            private GenericDispatcher(object filter)
            {
                _filter = filter;
            }

            public static GenericDispatcher GetFor<T>(IQueryFilterExpression<T> queryFilter)
            {
                return new GenericDispatcher(queryFilter);
            }

            public Expression CallGetFilterExpression(PropertyInfo property)
            {
                // We can assume this is safe, because the only call path is
                // compiler guaranteed to have the correct types. We just need
                // this because we can't have a generic parameter in a dictionary
                // when we don't know the type until runtime.
                const string methodName = nameof(IQueryFilterExpression<object>.GetForProperty);
                var method = _filter.GetType().GetMethod(methodName, new[] { typeof(PropertyInfo) });
                var expression = method.Invoke(_filter, new object[] { property }) as Expression;

                return MakeCorrectlyTyped(expression, property.PropertyType);
            }

            private static Expression MakeCorrectlyTyped(Expression expression, Type propertyType)
            {
                // expression is of type Expression<Func<propertyType, propertyType, bool>>
                if (expression.GetType().GenericTypeArguments[0].GenericTypeArguments[0] == propertyType)
                {
                    return expression;
                }

                var typeCorrector = new TypeCorrectingVisitor(propertyType);
                return typeCorrector.Visit(expression);
            }
        }
    }
}
