using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Pagination
{
    internal class PaginationContext
    {
        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int perPage)
        {
            var keyValuePairs = filters as IList<KeyValuePair<string, string>> ?? filters.ToList();

            var dictionary = keyValuePairs.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value.ToLowerInvariant());
            ClientFilters = dictionary;
            Page = GetNumber();
            PerPage = perPage;
        }

        public int Page { get; }

        public int PerPage { get; }

        public IDictionary<string, string> ClientFilters { get; }

        private int GetNumber()
        {
            if (!ClientFilters.ContainsKey(Constants.PageNumberQueryName))
            {
                return 0;
            }

            int result;
            var isNumber = int.TryParse(ClientFilters[Constants.PageNumberQueryName], out result);

            return isNumber ? result : 0;
        }
    }
}