using System.Collections.Generic;
using System.Linq;
using Saule.Http;

namespace Saule.Queries.Filtering
{
    internal class FilteringContext
    {
        public FilteringContext(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            Properties =
                from query in queryParams
                where query.Key.StartsWith(Constants.QueryNames.Filtering)
                let name = query.Key.Substring(Constants.QueryNames.Filtering.Length + 1)
                let value = query.Value
                select new FilteringProperty(name, value);
        }

        public IEnumerable<FilteringProperty> Properties { get; }

        public QueryFilterExpressionCollection QueryFilters { get; set; } = new QueryFilterExpressionCollection();

        public override string ToString()
        {
            return string.Join("&", Properties.Select(p => p.ToString()));
        }
    }
}
