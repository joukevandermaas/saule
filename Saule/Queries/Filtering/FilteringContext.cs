using System.Collections.Generic;
using System.Linq;
using Saule.Http;

namespace Saule.Queries.Filtering
{
    public class FilteringContext
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

        public QueryFilterExpressionCollection QueryFilters { get; internal set; } = new QueryFilterExpressionCollection();

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

        public override string ToString()
        {
            return string.Join("&", Properties.Select(p => p.ToString()));
        }
    }
}
