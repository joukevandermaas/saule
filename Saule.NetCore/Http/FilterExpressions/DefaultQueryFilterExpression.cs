using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Http.FilterExpressions
{
    /// <summary>
    /// Use for a simple equality comparison in query filters.
    /// </summary>
    /// <typeparam name="T">The type of the property that is being filtered on.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name must match first type name",
        Justification = "This is nicer")]
    public class DefaultQueryFilterExpression<T> : IQueryFilterExpression<T>
    {
        /// <summary>
        /// Returns the filtering expression for the given property.
        /// </summary>
        /// <param name="property">The property this queryFilter will be applied to.</param>
        /// <returns>A <see cref="Func{T1, T2, TResult}"/> that will be used to queryFilter the enumerable.</returns>
        public virtual Expression<Func<T, T, bool>> GetForProperty(PropertyInfo property)
        {
            var left = Expression.Parameter(typeof(T), "left");
            var right = Expression.Parameter(typeof(T), "right");
            return Expression.Lambda<Func<T, T, bool>>(Expression.Equal(left, right), left, right);
        }
    }
}