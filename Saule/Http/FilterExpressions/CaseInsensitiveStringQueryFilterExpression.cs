using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Http.FilterExpressions
{
    /// <summary>
    /// Use to make string comparisons in query filters case insensitive.
    /// </summary>
    public sealed class CaseInsensitiveStringQueryFilterExpression : IQueryFilterExpression<string>
    {
        /// <summary>
        /// Returns the filtering expression for the given property.
        /// </summary>
        /// <param name="property">The property this queryFilter will be applied to.</param>
        /// <returns>A <see cref="Func{T1, T2, TResult}"/> that will be used to queryFilter the enumerable.</returns>
        public Expression<Func<string, string, bool>> GetForProperty(PropertyInfo property)
        {
            return (left, right) => left.Equals(right, StringComparison.OrdinalIgnoreCase);
        }
    }
}