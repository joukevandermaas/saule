using System;
using System.Collections.Generic;
using System.Linq;
using Saule.Queries.Filtering;
using Saule.Queries.Including;
using Saule.Queries.Pagination;
using Saule.Queries.Sorting;

namespace Saule.Queries
{
    /// <summary>
    /// Context with all Json Api operations for current request
    /// </summary>
    public class QueryContext
    {
        /// <summary>
        /// Gets pagination context
        /// </summary>
        public PaginationContext Pagination { get; internal set; }

        /// <summary>
        /// Gets sorting context
        /// </summary>
        public SortingContext Sorting { get; internal set; }

        /// <summary>
        /// Gets filtering context
        /// </summary>
        public FilteringContext Filtering { get; internal set; }

        /// <summary>
        /// Gets including context
        /// </summary>
        public IncludingContext Including { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether that query parameters
        /// will be handled by action itself or Saule should handle them
        /// </summary>
        internal bool IsManuallyHandledQuery { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new List<string>
            {
                Pagination?.ToString(),
                Sorting?.ToString(),
                Filtering?.ToString(),
                Including?.ToString()
            };

            return string.Join("&", result.Where(c => !string.IsNullOrEmpty(c)));
        }
    }
}
