using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Saule.Http.FilterExpressions
{
    /// <summary>
    /// Use to make string comparisons in query filters case insensitive.
    /// </summary>
    public sealed class StringMultiQueryFilterExpression : DefaultQueryFilterExpression<string>
    {
        /// <summary>
        /// Returns the filtering expression for the given property.
        /// </summary>
        /// <param name="property">The property this queryFilter will be applied to.</param>
        /// <returns>A <see cref="Func{T1, T2, TResult}"/> that will be used to queryFilter the enumerable.</returns>
        public override Expression<Func<string, string, bool>> GetForProperty(PropertyInfo property)
        {
            return (prop, filter) => CheckMultiple(prop, filter);
        }

        private bool CheckMultiple(string prop, string filter)
        {
            List<string> filterValues = new List<string>();

            var match = new Regex("\"(.+?)\"|(\\w+(?=,|$))").Matches(filter);
            foreach (Capture matchCapture in match)
            {
                var value = matchCapture.Value;

                // Fix for regex matching including the duoble quotes
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                filterValues.Add(value);
            }

            return filterValues.Contains(prop);
        }
    }
}