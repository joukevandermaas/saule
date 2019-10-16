using System.Collections;
using System.Collections.Generic;

namespace Saule.Queries.Pagination
{
    /// <summary>
    /// Simple implementation of IPagedResult
    /// </summary>
    /// <typeparam name="T">Type of collection item</typeparam>
    public class PagedResult<T> : IPagedResult
    {
        /// <inheritdoc/>
        public int TotalResultsCount { get; set; }

        IEnumerable IPagedResult.Items { get { return Data; } }

        /// <summary>
        /// Gets or sets gets data items as IList
        /// </summary>
        public IList<T> Data { get; set; }
    }
}
