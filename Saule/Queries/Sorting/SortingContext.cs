using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Sorting
{
    internal class SortingContext
    {
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

        public IEnumerable<SortingProperty> Properties { get; }

        public override string ToString()
        {
            return $"sort={string.Join(",", Properties)}";
        }
    }
}
