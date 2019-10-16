using System.Collections;

namespace Saule.Queries.Pagination
{
    /// <summary>
    /// Paged result that can be used to tell Saule how many pages we have
    /// </summary>
    public interface IPagedResult
    {
        /// <summary>
        /// Gets total count of items across all pages
        /// </summary>
        int TotalResultsCount { get; }

        /// <summary>
        /// Gets data items
        /// </summary>
        IEnumerable Items { get; }
    }
}
