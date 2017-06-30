using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Pagination
{
    internal class PaginationContext
    {
        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int pageSizeDefault)
            : this(filters, pageSizeDefault, false)
        {
        }

        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int pageSizeDefault, bool acceptPageSizeQuery)
        {
            var keyValuePairs = filters as IList<KeyValuePair<string, string>> ?? filters.ToList();

            var dictionary = keyValuePairs.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value.ToLowerInvariant());
            ClientFilters = dictionary;
            Page = GetNumber();
            PerPage = GetSize(pageSizeDefault, acceptPageSizeQuery);
        }

        public int Page { get; }

        public int PerPage { get; }

        public IDictionary<string, string> ClientFilters { get; }

        public override string ToString()
        {
            return $"page[number]={Page}&page[size]={PerPage}";
        }

        private int GetNumber()
        {
            return ClientFilters.GetInt(Constants.QueryNames.PageNumber, 0);
        }

        private int GetSize(int defaultSize, bool checkQueryParam)
        {
            return checkQueryParam ? ClientFilters.GetInt(Constants.QueryNames.PageSize, defaultSize) : defaultSize;
        }
    }
}