using System.Collections.Generic;
using System.Linq;
using Saule.Http;

namespace Saule.Queries.Filtering
{
    /// <summary>
    /// Context for filtering operations
    /// </summary>
    public class FilterContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterContext"/> class.
        /// </summary>
        /// <param name="queryParams">query string that might contain Filter keyword</param>
        public FilterContext(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            Properties =
                from query in queryParams
                where query.Key.StartsWith(Constants.QueryNames.Filtering)
                let name = query.Key.Substring(Constants.QueryNames.Filtering.Length + 1)
                let value = query.Value
                select new FilterProperty(name, value);
        }

        /// <summary>
        /// Gets filtering properties
        /// </summary>
        public IEnumerable<FilterProperty> Properties { get; }

        /// <summary>
        /// Gets custom query filters
        /// </summary>
        public QueryFilterExpressionCollection QueryFilters { get; internal set; } = new QueryFilterExpressionCollection();

        /// <summary>
        /// checking that specified property exists and returns converted value for it
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        /// <returns>true if property is specified. Otherwise false</returns>
        public bool TryGetValue<T>(string name, out List<T> value)
        {
            var property = Properties.FirstOrDefault(p => p.Name == name);
            value = new List<T>();
            if (property == null)
            {
                return false;
            }

            value = property.Values.Select(v => (T) Lambda.TryConvert(v, typeof(T))).ToList();
            return true;
        }

        /// <summary>
        /// checking that specified property exists and returns converted value for it
        /// </summary>
        /// <typeparam name="T">Type of property</typeparam>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        /// <returns>true if property is specified. Otherwise false</returns>
        public bool TryGetValue<T>(string name, out T value)
        {
            var property = Properties.FirstOrDefault(p => p.Name == name);
            if (property == null)
            {
                value = default(T);
                return false;
            }

            value = (T)Lambda.TryConvert(property.Value, typeof(T));
            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Join("&", Properties.Select(p => p.ToString()));
        }
    }
}
