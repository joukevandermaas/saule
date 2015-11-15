using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Sorting
{
    internal class SortingContext
    {
        public SortingContext(IEnumerable<KeyValuePair<string, string>> filters)
        {
            var dict = filters.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (dict.ContainsKey(Constants.SortingQueryName))
            {
                var props = dict[Constants.SortingQueryName].Split(',').Select(s => s.Trim());

                Properties = props.Select(p => new SortingProperty(p));
            }
        }

        public IEnumerable<SortingProperty> Properties { get; }
    }
}
