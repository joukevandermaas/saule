using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Http.FilterExpressions
{
    /// <summary>
    /// Use to specify a lambda that will be used in query filtering.
    /// </summary>
    /// <typeparam name="T">The type of the property that is being filtered on.</typeparam>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name must match first type name",
        Justification = "This is nicer")]
    internal class LambdaQueryFilterExpression<T> : IQueryFilterExpression<T>
    {
        private readonly Expression<Func<T, T, bool>> _expression;

        public LambdaQueryFilterExpression(Expression<Func<T, T, bool>> expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// Returns the filtering expression for the given property.
        /// </summary>
        /// <param name="property">The property this queryFilter will be applied to.</param>
        /// <returns>A <see cref="Func{T1, T2, TResult}"/> that will be used to queryFilter the enumerable.</returns>
        public Expression<Func<T, T, bool>> GetForProperty(PropertyInfo property)
        {
            return _expression;
        }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}