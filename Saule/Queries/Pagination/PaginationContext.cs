using System.Collections.Generic;
using System.Linq;

namespace Saule.Queries.Pagination
{
    /// <summary>
    /// Context for pagination
    /// </summary>
    public class PaginationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationContext"/> class.
        /// </summary>
        /// <param name="filters">query string that might contain Page keyword</param>
        /// <param name="pageSizeDefault">default page size</param>
        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int? pageSizeDefault)
            : this(filters, pageSizeDefault, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationContext"/> class.
        /// </summary>
        /// <param name="filters">query string that might contain Page keyword</param>
        /// <param name="pageSizeDefault">default page size</param>
        /// <param name="pageSizeLimit">maximum page size</param>
        /// <param name="firstPageNumber">the first page number</param>
        public PaginationContext(IEnumerable<KeyValuePair<string, string>> filters, int? pageSizeDefault, int? pageSizeLimit, int? firstPageNumber)
        {
            var keyValuePairs = filters as IList<KeyValuePair<string, string>> ?? filters.ToList();

            var dictionary = keyValuePairs.ToDictionary(kv => kv.Key.ToLowerInvariant(), kv => kv.Value.ToLowerInvariant());
            ClientFilters = dictionary;
            Page = GetNumber();
            PerPage = GetSize(pageSizeDefault);
            PageSizeLimit = pageSizeLimit;
            FirstPageNumber = firstPageNumber ?? 0;
        }

        /// <summary>
        /// Gets page number
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets page size
        /// </summary>
        public int? PerPage { get; internal set; }

        /// <summary>
        /// Gets maximum page size
        /// </summary>
        public int? PageSizeLimit { get; internal set; }

        /// <summary>
        /// Gets the first page that. Default value is 0
        /// </summary>
        public int FirstPageNumber { get; internal set; }

        /// <summary>
        /// Gets client filters
        /// </summary>
        public IDictionary<string, string> ClientFilters { get; }

        /// <inheritdoc/>
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