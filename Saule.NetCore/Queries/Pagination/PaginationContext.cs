using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Pagination
{
    internal class PaginationContext
    {
        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int? pageSizeDefault)
            : this(filters, pageSizeDefault, null)
        {
        }

        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int? pageSizeDefault, int? pageSizeLimit)
        {
            var keyValuePairs = filters as IList<KeyValuePair<string, string>> ?? filters.ToList();

            var dictionary = keyValuePairs.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value.ToLowerInvariant());
            ClientFilters = dictionary;
            Page = GetNumber();
            PerPage = GetSize(pageSizeDefault);
            PageSizeLimit = pageSizeLimit;
        }

        public int Page { get; }

        public int? PerPage { get; set; }

        public int? PageSizeLimit { get; set; }

        public IDictionary<string, string> ClientFilters { get; }

        public override string ToString()
        {
            string pageNum = $"page[number]={Page}";
            string pageSize = PerPage.HasValue ? $"&page[size]={PerPage}" : string.Empty;
            return pageNum + pageSize;
        }

        private int GetNumber()
        {
            return ClientFilters.GetInt(Constants.QueryNames.PageNumber, 0);
        }

        private int? GetSize(int? defaultSize)
        {
            int? queryPageSize = ClientFilters.GetInt(Constants.QueryNames.PageSize);
            return queryPageSize ?? defaultSize;
        }
    }
}