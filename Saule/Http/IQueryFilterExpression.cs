using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Http
{
    /// <summary>
    /// Represents an expression that is applied to enumerables when filtering them.
    /// </summary>
    /// <typeparam name="T">The type of the property that is being filtered on.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name must match first type name",
        Justification = "This is nicer")]
    public interface IQueryFilterExpression<T>
    {
        /// <summary>
        /// Returns the filtering expression for the given property.
        /// </summary>
        /// <param name="property">The property this filter will be applied to.</param>
        /// <returns>A <see cref="Func{T1, T2, TResult}"/> that will be used to filter the enumerable.</returns>
        Expression<Func<T, T, bool>> GetForProperty(PropertyInfo property);
    }
}