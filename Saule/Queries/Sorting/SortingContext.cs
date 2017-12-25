using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Sorting
{
    /// <summary>
    /// Context for sorting operations
    /// </summary>
    public class SortingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortingContext"/> class.
        /// </summary>
        /// <param name="filters">query string that might contain Sorting keyword</param>
        public SortingContext(IEnumerable<KeyValuePair<string, string>> filters)
        {
            var dict = filters.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (dict.ContainsKey(Constants.QueryNames.Sorting))
            {
                var props = dict[Constants.QueryNames.Sorting].Split(',').Select(s => s.Trim());

                Properties = props.Select(p => new SortingProperty(p));
            }
            else
            {
                Properties = new SortingProperty[0];
            }
        }

        /// <summary>
        /// Gets sorting properties
        /// </summary>
        public IEnumerable<SortingProperty> Properties { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"sort={string.Join(",", Properties)}";
        }
    }
}
