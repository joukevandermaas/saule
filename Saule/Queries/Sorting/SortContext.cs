using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Sorting
{
    /// <summary>
    /// Context for sorting operations
    /// </summary>
    public class SortContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortContext"/> class.
        /// </summary>
        /// <param name="filters">query string that might contain Sorting keyword</param>
        public SortContext(IEnumerable<KeyValuePair<string, string>> filters)
        {
            var dict = filters.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (dict.ContainsKey(Constants.QueryNames.Sorting))
            {
                var props = dict[Constants.QueryNames.Sorting].Split(',').Select(s => s.Trim());

                Properties = props.Select(p => new SortProperty(p));
            }
            else
            {
                Properties = new SortProperty[0];
            }
        }

        /// <summary>
        /// Gets sorting properties
        /// </summary>
        public IEnumerable<SortProperty> Properties { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"sort={string.Join(",", Properties)}";
        }
    }
}
