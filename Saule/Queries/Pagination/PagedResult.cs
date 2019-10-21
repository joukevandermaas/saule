using System.Collections;
using System.Collections.Generic;

namespace Saule.Queries.Pagination
{
    /// <summary>
    /// Simple implementation of IPagedResult
    /// </summary>
    /// <typeparam name="T">Type of collection item</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets or sets total count of items across all pages
        /// </summary>
        public int TotalResultsCount { get; set; }

        /// <summary>
        /// Gets or sets gets data items as IList
        /// </summary>
        public IList<T> Data { get; set; }

        internal IEnumerable Items
        {
            get { return Data; }
        }
    }
}
