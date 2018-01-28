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
        /// Gets sort context
        /// </summary>
        public SortContext Sort { get; internal set; }

        /// <summary>
        /// Gets filter context
        /// </summary>
        public FilterContext Filter { get; internal set; }

        /// <summary>
        /// Gets include context
        /// </summary>
        public IncludeContext Include { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether that query parameters
        /// will be handled by action itself or Saule should handle them
        /// </summary>
        internal bool IsHandledQuery { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new List<string>
            {
                Pagination?.ToString(),
                Sort?.ToString(),
                Filter?.ToString(),
                Include?.ToString()
            };

            return string.Join("&", result.Where(c => !string.IsNullOrEmpty(c)));
        }
    }
}
